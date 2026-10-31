using System;
using System.Globalization;
using DotNetNuke.Entities.Modules;
using BCC.Modules.BCCForms.Components;
using System.Web.UI.WebControls;

namespace BCC.Modules.BCCForms
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   Module Settings
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : ModuleSettingsBase
    {
        public override void LoadSettings()
        {
            txtUserRecordQuota.Text = Settings[SettingName.UserRecordQuota].AsString();
            chkEditOwnData.Checked = Convert.ToBoolean(Settings[SettingName.EditOnlyOwnItems]);           
            chkInputFiltering.Checked = Convert.ToBoolean(Settings[SettingName.ForceInputFiltering]);
            chkDisplayColumns.Checked =
                Convert.ToBoolean(!Settings[SettingName.ShowAllColumnsForAdmins].AsBoolean(true));
            chkPrivateColumns.Checked =
                Convert.ToBoolean(!Settings[SettingName.EditPrivateColumnsForAdmins].AsBoolean(true));
            chkHideSystemColumns.Checked = Convert.ToBoolean(!Settings[SettingName.ShowSystemColumns].AsBoolean());
            var current = new Components.Settings(Settings);
            foreach (CaptchaMode mode in Enum.GetValues(typeof (CaptchaMode)))
            {
                ddlCaptcha.Items.Add(new ListItem(LocalizeText("CaptchaMode_" + mode), mode.ToString()));
            }
            ddlCaptcha.SelectedValue = current.CaptchaMode.ToString();
            txtMinScore.Text = current.ReCaptchaMinScore.ToString("0.0#", CultureInfo.InvariantCulture);
            chkHoneypot.Checked = current.UseHoneypot;
            txtSiteKey.Text = Settings[SettingName.ReCaptchaSiteKey].AsString();
            txtSecretKey.Text = Settings[SettingName.ReCaptchaSecretKey].AsString();
        }        

        public override void UpdateSettings()
        {
            var mc = ModuleController.Instance;
            mc.UpdateModuleSetting(ModuleId, SettingName.UserRecordQuota, txtUserRecordQuota.Text);
            mc.UpdateModuleSetting(ModuleId, SettingName.EditOnlyOwnItems, chkEditOwnData.Checked.ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.ForceInputFiltering, chkInputFiltering.Checked.ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.ShowAllColumnsForAdmins,
                                   (!(chkDisplayColumns.Checked)).ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.EditPrivateColumnsForAdmins,
                                   (!(chkPrivateColumns.Checked)).ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.ShowSystemColumns, (!(chkHideSystemColumns.Checked)).ToString());
            CaptchaMode captchaMode;
            if (!Enum.TryParse(ddlCaptcha.SelectedValue, out captchaMode)) captchaMode = CaptchaMode.None;
            mc.UpdateModuleSetting(ModuleId, SettingName.CaptchaMode, captchaMode.ToString());
            // keep the legacy pair in step so an older build of the module still reads a sensible value
            mc.UpdateModuleSetting(ModuleId, SettingName.ForceCaptchaForAnonymous,
                                   (captchaMode != CaptchaMode.None).ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.PreferReCaptcha,
                                   (captchaMode == CaptchaMode.ReCaptchaV2).ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.ReCaptchaMinScore,
                                   Components.Settings.ParseScore(txtMinScore.Text.Trim())
                                       .ToString("0.0#", CultureInfo.InvariantCulture));
            mc.UpdateModuleSetting(ModuleId, SettingName.UseHoneypot, chkHoneypot.Checked.ToString());
            mc.UpdateModuleSetting(ModuleId, SettingName.ReCaptchaSiteKey, txtSiteKey.Text.Trim());
            mc.UpdateModuleSetting(ModuleId, SettingName.ReCaptchaSecretKey, txtSecretKey.Text.Trim());
        }        
    }
}