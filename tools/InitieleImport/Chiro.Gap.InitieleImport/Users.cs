using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.InitieleImport
{
	public static class Users
	{
		public class LoginInfo
		{
			public string Login { get; set; }
			public string StamNr { get; set; }
		}

		public static IEnumerable<LoginInfo> Lijst = new LoginInfo[]
		                                             	{
			new LoginInfo { Login = @"CHIROPUBLIC\lemmest", StamNr = "AG /0907" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\catbroe", StamNr = "BJ /0204" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\scheedo", StamNr = "BM /0317" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\verdyge", StamNr = "KG /0101" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\francke", StamNr = "KG /0101" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\remijdi", StamNr = "KG /0510" }, 
			new LoginInfo { Login = @"KIPDORP\BouweHa", StamNr = "KJ /0202" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\kustera", StamNr = "KJ /0504" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\menheha", StamNr = "KM /0206" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\jorisru", StamNr = "KM /0509" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\laerhni", StamNr = "LEG/0510" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\baumasi", StamNr = "LEG/0608" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\symonti", StamNr = "LEJ/0109" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\verlaan", StamNr = "LG /0203" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\straeje", StamNr = "LJ /0807" }, 
			new LoginInfo { Login = @"KIPDORP\meersko", StamNr = "MG /0111" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\MeersKo2", StamNr = "MG /0111" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vervljo", StamNr = "MG /0113" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\testjohan", StamNr = "MG /0113" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\testjohan", StamNr = "MJ /0108" }, 
			new LoginInfo { Login = @"KIPDORP\gouweme", StamNr = "MJ /0108" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vercaro", StamNr = "MJ /0109" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\jacobis", StamNr = "MM /0109" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\beullma", StamNr = "MM /0509" }, 
			new LoginInfo { Login = @"KIPDORP\EelenJi", StamNr = "MM /0704" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\brackar", StamNr = "OG /1203" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\ryonced", StamNr = "OG /1511" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\daneewi", StamNr = "OG /3315" }, 
			new LoginInfo { Login = @"KIPDORP\HauweKr", StamNr = "OJ /2110" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\spoorst", StamNr = "OJ /2202" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\smetwal", StamNr = "OJ /2310" }, 
			new LoginInfo { Login = @"KIPDORP\SmetWal", StamNr = "OJ /2310" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\kerckpa", StamNr = "OM /1412" }, 
			new LoginInfo { Login = @"KIPDORP\bertepe", StamNr = "WG /0206" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vanrojo", StamNr = "WG /0206" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\testpeter", StamNr = "WG /0206" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\przybko", StamNr = "WG /1013" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vandase", StamNr = "WG /1109" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vandest", StamNr = "WG /1401" }, 
			new LoginInfo { Login = @"KIPDORP\michama", StamNr = "WG /1404" }, 
			new LoginInfo { Login = @"KIPDORP\booneba", StamNr = "WG /0901" },
			new LoginInfo { Login = @"CHIROPUBLIC\vandeda", StamNr = "WJ /1908" }, 
			new LoginInfo { Login = @"CHIROPUBLIC\vandesa", StamNr = "WM /1803" },
			new LoginInfo { Login = @"KIPDORP\perpema", StamNr = "OJ /2306" },
			new LoginInfo { Login = @"CHIROPUBLIC\maesjas", StamNr = "BG /0205" }
								};

	}
}
