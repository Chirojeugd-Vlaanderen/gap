﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.WebApp.Properties
{
	// This class allows you to handle specific events on the settings class:
	//  The SettingChanging event is raised before a setting's value is changed.
	//  The PropertyChanged event is raised after a setting's value is changed.
	//  The SettingsLoaded event is raised after the setting values are loaded.
	//  The SettingsSaving event is raised before the setting values are saved.
	internal sealed partial class Settings
	{
		private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
		{
			// Add code to handle the SettingChangingEvent event here.
		}

		private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Add code to handle the SettingsSaving event here.
		}
	}
}