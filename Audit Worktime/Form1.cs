using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.AccessControl;
using System.DirectoryServices;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Audit_Worktime
{
    public partial class Form1 : Form
    {
        enum POLICY_INFORMATION_CLASS
        {
            PolicyAuditLogInformation = 1,
            PolicyAuditEventsInformation,
            PolicyPrimaryDomainInformation,
            PolicyPdAccountInformation,
            PolicyAccountDomainInformation,
            PolicyLsaServerRoleInformation,
            PolicyReplicaSourceInformation,
            PolicyDefaultQuotaInformation,
            PolicyModificationInformation,
            PolicyAuditFullSetInformation,
            PolicyAuditFullQueryInformation,
            PolicyDnsDomainInformation
        }

        public enum POLICY_AUDIT_EVENT_TYPE
        {
            AuditCategorySystem,
            AuditCategoryLogon,
            AuditCategoryObjectAccess,
            AuditCategoryPrivilegeUse,
            AuditCategoryDetailedTracking,
            AuditCategoryPolicyChange,
            AuditCategoryAccountManagement,
            AuditCategoryDirectoryServiceAccess,
            AuditCategoryAccountLogon
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PAUDIT_POLICY_INFORMATION
        {
            public Guid AuditSubCategoryGuid;
            public uint AuditingInformation;
            public Guid AuditCategoryGuid;
            //IntPtr pointer;
        }

        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        public struct GUID
        {
            public Int32 Data1;
            public Int16 Data2;
            public Int16 Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Data4;
        }

        public enum AuditingInformation
        {
            POLICY_AUDIT_EVENT_UNCHANGED=0,
            POLICY_AUDIT_EVENT_SUCCESS=1,
            POLICY_AUDIT_EVENT_FAILURE=2,
            POLICY_AUDIT_EVENT_NONE=4
        }

        public static GUID ToGUID(Guid guid)
        {
            GUID newGuid = new GUID();
            byte[] guidData = guid.ToByteArray();
            newGuid.Data1 = BitConverter.ToInt32(guidData, 0);
            newGuid.Data2 = BitConverter.ToInt16(guidData, 4);
            newGuid.Data3 = BitConverter.ToInt16(guidData, 6);
            newGuid.Data4 = new byte[8];
            Array.Copy(guidData, 8, newGuid.Data4, 0, 8);
            return newGuid;
        }

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig=true)]
        public static extern bool AuditQuerySystemPolicy(
            IntPtr pSubCategoryGuids,
            UInt32 PolicyCount,
            out IntPtr ppAuditPolicy);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditEnumerateSubCategories(
            IntPtr pAuditCategoryGuid,
            bool bRetrieveAllSubCategories,
            out IntPtr ppAuditSubCategoriesArray,
            out uint pCountReturned);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditSetSystemPolicy(
            IntPtr ppAuditPolicy,
            UInt32 PolicyCount);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern void AuditFree(
            IntPtr Buffer);

        private DateTime selected_date = DateTime.Today;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr pGUID=IntPtr.Zero;
            IntPtr subCats = IntPtr.Zero;
            IntPtr pAuditPolicy = IntPtr.Zero;
            int last_API_err = 0;

            this.Text = "Audit Worktime v." + Application.ProductVersion.Substring(0, Application.ProductVersion.Length-4);

            try
            {
                monthCalendar1.TodayDate = DateTime.Today;

                bool audit_res = false;
                //Audit logon category Audit_Logon=GUID 69979849-797a-11d9-bed3-505054503030
                //auditpol.exe /get /category:{69979849-797a-11d9-bed3-505054503030}

                //subcategory logon=0cce9215-69ae-11d9-bed3-505054503030
                //subcategory logoff=0cce9216-69ae-11d9-bed3-505054503030
                Guid audit_logon_guid = new Guid("69979849-797a-11d9-bed3-505054503030");
                GUID myGUID = ToGUID(audit_logon_guid);
                pGUID = Marshal.AllocHGlobal(Marshal.SizeOf(myGUID));
                Marshal.StructureToPtr(myGUID, pGUID, true);

                pAuditPolicy = Marshal.AllocHGlobal(9*Marshal.SizeOf(typeof(PAUDIT_POLICY_INFORMATION)));
                uint nSubCats = 0;

                try
                {
                    if (!(audit_res = AuditEnumerateSubCategories(pGUID, false, out subCats, out nSubCats)))
                    {
                        last_API_err = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(last_API_err);
                    }
                }
                catch (EntryPointNotFoundException ex)
                { }

                try
                {
                    if (!(audit_res = AuditQuerySystemPolicy(subCats, nSubCats, out pAuditPolicy)))
                    {
                        last_API_err = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(last_API_err);
                    }
                }
                catch (EntryPointNotFoundException ex)
                { }

                int audit_type_size = Marshal.SizeOf(typeof(PAUDIT_POLICY_INFORMATION));
                PAUDIT_POLICY_INFORMATION[] str_info_arr=new PAUDIT_POLICY_INFORMATION[9];
                
                for (int i = 0; i < 9; i++)
                {
                    str_info_arr[i] = (PAUDIT_POLICY_INFORMATION)Marshal.PtrToStructure(IntPtr.Add(pAuditPolicy, i * audit_type_size), typeof(PAUDIT_POLICY_INFORMATION));
                }
                
                AuditingInformation audit_flags;

                //check audit logon subcategory policy
                audit_flags = (AuditingInformation)str_info_arr[0].AuditingInformation;
                try
                {
                    if (!audit_flags.HasFlag(AuditingInformation.POLICY_AUDIT_EVENT_SUCCESS))
                    {
                        //Success logon audit policy disabled
                        if (MessageBox.Show("Audit policy for Success logon is disabled(use 'secpol.msc' command in console), enable it?", "Enable success audit logon?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PrivilegeManager.SetPrivilege(Process.GetCurrentProcess().Handle, PrivilegeManager.SE_SECURITY_NAME);

                            str_info_arr[0].AuditingInformation |= (uint)AuditingInformation.POLICY_AUDIT_EVENT_SUCCESS;
                            Marshal.StructureToPtr(str_info_arr[0], pAuditPolicy, false);

                            if (!(audit_res = AuditSetSystemPolicy(pAuditPolicy, nSubCats)))
                            {
                                last_API_err = Marshal.GetLastWin32Error();
                                throw new System.ComponentModel.Win32Exception(last_API_err);
                            }
                        }
                    }
                }
                catch (EntryPointNotFoundException ex)
                { }

                //check audit logoff subcategory policy
                audit_flags = (AuditingInformation)str_info_arr[1].AuditingInformation;
                try
                {
                    if (!audit_flags.HasFlag(AuditingInformation.POLICY_AUDIT_EVENT_SUCCESS))
                    {
                        //Success logoff audit policy disabled
                        if (MessageBox.Show("Audit policy for Success logoff is disabled(use secpol.msc console), enable it?", "Enable success audit logoff?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PrivilegeManager.SetPrivilege(Process.GetCurrentProcess().Handle, PrivilegeManager.SE_SECURITY_NAME);

                            str_info_arr[1].AuditingInformation |= (uint)AuditingInformation.POLICY_AUDIT_EVENT_SUCCESS;

                            for (int i = 0; i < str_info_arr.Length; i++)
                            {
                                byte[] curr_audit_bytes = getBytes(str_info_arr[i]);
                                Marshal.Copy(curr_audit_bytes, 0, IntPtr.Add(pAuditPolicy, i * audit_type_size), audit_type_size);
                            }

                            if (!(audit_res = AuditSetSystemPolicy(pAuditPolicy, nSubCats)))
                            {
                                last_API_err = Marshal.GetLastWin32Error();
                                throw new System.ComponentModel.Win32Exception(last_API_err);
                            }
                        }
                    }
                }
                catch (EntryPointNotFoundException ex)
                { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + last_API_err.ToString()+": "+ ex.Message);
                return;
            }
            finally
            {
                Marshal.FreeHGlobal(pGUID);
                Marshal.FreeHGlobal(subCats);
                try
                {
                    AuditFree(pAuditPolicy);
                }
                catch (EntryPointNotFoundException ex)
                { }
            }
        }

        private void btnGetDayHours_Click(object sender, EventArgs e)
        {
            txtHoursDay.Text = "0";
            try
            {
                EventLog user_log = new EventLog("Security");
                EventLogEntryCollection sec_events = user_log.Entries;
                btnGetDayHours.Enabled = false;

                if (sec_events.Count < 1)
                {
                    MessageBox.Show("No events in Security Event Log! Please enable audit security events.", "Audit Worktime", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                txtHoursDay.Text = GetDayHours(sec_events, selected_date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Audit Worktime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                btnGetDayHours.Enabled = true;
            }
        }

        string GetDayHours(EventLogEntryCollection sec_events, DateTime curr_selected_date)
        {
            string user_name = Environment.UserDomainName + @"\" + Environment.UserName;
            DateTime curr_date = new DateTime();
            DateTime start_time = new DateTime(), end_time = new DateTime();
            bool start_time_first = false;
            bool date_today = false;
            string res="0";

            if (curr_selected_date.Date == DateTime.Today)
            {
                curr_date = DateTime.Now;
                start_time = DateTime.Today;
                end_time = DateTime.Now;
                date_today = true;
            }
            else
            {
                curr_date = curr_selected_date.Date;
                start_time = curr_selected_date.Date.AddHours(10.0); //start work usually at 10.00
                end_time = curr_selected_date.Date.AddHours(19.0);//end work last time at 19.00
            }

            foreach (EventLogEntry curr_event in sec_events.Cast<EventLogEntry>().OrderBy<EventLogEntry, DateTime>(o => o.TimeGenerated))
            {
                //EventID(InstanceId)=528 - Succesful user logon (Win7 - 4624)
                //EventId=551 - User initiated logoff (Win7 - 4647)
                if (curr_event.EntryType == EventLogEntryType.SuccessAudit && (curr_event.InstanceId == 528 || curr_event.InstanceId == 551
                    || curr_event.InstanceId == 4624 || curr_event.InstanceId == 4647) && curr_event.TimeGenerated.Date == curr_date.Date)
                {
                    if (curr_event.UserName == user_name ||
                       (curr_event.ReplacementStrings.Length > 2 && curr_event.ReplacementStrings[2] + @"\" + curr_event.ReplacementStrings[1] == user_name) || 
                       (curr_event.ReplacementStrings.Length > 6 && curr_event.ReplacementStrings[6] + @"\" + curr_event.ReplacementStrings[5] == user_name))
                    {
                        if ((curr_event.InstanceId == 528 || curr_event.InstanceId==4624) && !start_time_first)
                        {
                            start_time = curr_event.TimeGenerated;
                            start_time_first = true;
                        }
                        else if (curr_event.InstanceId == 551 || curr_event.InstanceId == 4647)
                        {
                            if (!date_today) end_time = curr_event.TimeGenerated;
                        }
                        double work_hours = (end_time - start_time).TotalHours;
                        res = work_hours.ToString("F1");
                    }
                }
            }

            return res;
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            //MessageBox.Show(e.End.Date.ToLongDateString());
            if (!backgroundWorkerParseEvents.IsBusy) selected_date = e.End.Date;
        }

        private void btnGetMonthHours_Click(object sender, EventArgs e)
        {
            txtHoursMonth.Text = "";
            btnGetMonthHours.Enabled = false;
            btnGetDayHours.Enabled = false;

            try
            {
                EventLog user_log = new EventLog("Security");
                EventLogEntryCollection sec_events = user_log.Entries;

                if (!backgroundWorkerParseEvents.IsBusy)
                {
                    backgroundWorkerParseEvents.RunWorkerAsync(sec_events);
                }
            }
            catch (Exception ex)
            {
                btnGetMonthHours.Enabled = true;
                btnGetDayHours.Enabled = true;
                MessageBox.Show("Error " + ex.Message, "Audit Worktime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        static byte[] getBytes(object str)
        {
            int size=0;
            if (str.GetType().Name == "PAUDIT_POLICY_INFORMATION[]")
            {
                size = (str as Array).Length * Marshal.SizeOf(typeof(PAUDIT_POLICY_INFORMATION)); 
            }
            else
            {
                size = Marshal.SizeOf(str);
            }

            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            if (str.GetType().Name == "PAUDIT_POLICY_INFORMATION[]")
            {
                Marshal.StructureToPtr((str as Array).GetValue(0), ptr, false);
            }
            else
            {
                Marshal.StructureToPtr(str, ptr, false);
            }
            
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        static T fromBytes<T>(byte[] arr)
        {
            T str = default(T);

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        private void backgroundWorkerParseEvents_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            EventLogEntryCollection sec_events = e.Argument as EventLogEntryCollection;
            string text_res = "";
            float total_hours = 0;
            float curr_day_hours = 0;

            for (int i = 1; i <= selected_date.Day; i++)
            {
                DateTime curr_date = new DateTime(selected_date.Year, selected_date.Month, i);
                curr_day_hours = float.Parse(GetDayHours(sec_events, curr_date));
                total_hours += curr_day_hours;
                text_res += curr_date.ToShortDateString() + " " + curr_day_hours.ToString("F1") + Environment.NewLine;

                worker.ReportProgress((int)((double)i / selected_date.Day * 100));
            }
            text_res += "Total month hours: " + total_hours.ToString("F1");
            e.Result = text_res;
        }

        private void backgroundWorkerParseEvents_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarEvents.Value = e.ProgressPercentage;
        }

        private void backgroundWorkerParseEvents_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtHoursMonth.Text = e.Result.ToString();
            txtHoursMonth.SelectionStart = e.Result.ToString().Length;
            txtHoursMonth.ScrollToCaret();
            btnGetMonthHours.Enabled = true;
            btnGetDayHours.Enabled = true;
        }
    }
}
