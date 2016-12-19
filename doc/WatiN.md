Links (extern)
--------------

\* WatiN homepage: [\* WatiN API documentatie:
\[http://watin.sourceforge.net/doc/index.html](http://watin.sourceforge.net/])

\* Another look at WatiN: [\* WatiN Test Recorder:
\[http://watintestrecord.sourceforge.net/](http://www.richardlawrence.info/2009/01/24/another-look-at-watin/])

**** Voor diegenen die willen testen:
Versie [TestRecorderB](TestRecorderB.md)0603.msi is geinstalleerd op devserver

WatiN
-----

**** Problemen en Oplossingen

```
**Bij het opstarten van de testen**

-- Error message:

Test method Chiro.WatiN.Test.FirstWatInTest.InvoegenPersonen threw
exception: WatiN.Core.Exceptions.TimeoutException: Timeout while
Internet Explorer state not complete.

-- stacktrace:

WatiN.Core.UtilityClasses.TryFuncUntilTimeOut.ThrowTimeOutException(Exception
lastException, String message)
WatiN.Core.UtilityClasses.TryFuncUntilTimeOut.HandleTimeOut()
WatiN.Core.UtilityClasses.TryFuncUntilTimeOut.Try\[T\](DoFunc@1 func)
WatiN.Core.WaitForCompleteBase.WaitUntil(DoFunc@1 waitWhile,
[BuildTimeOutExceptionMessage](BuildTimeOutExceptionMessage.md) exceptionMessage)
WatiN.Core.Native.InternetExplorer.WaitForComplete.WaitWhileIEReadyStateNotComplete(IWebBrowser2
ie)
WatiN.Core.Native.InternetExplorer.IEWaitForComplete.DoWait()
WatiN.Core.DomContainer.WaitForComplete(IWait waitForComplete)
WatiN.Core.IE.WaitForComplete(Int32 waitForCompleteTimeOut)
WatiN.Core.DomContainer.WaitForComplete()
WatiN.Core.Browser.GoTo(Uri url)
WatiN.Core.Browser.GoTo(String url)
Chiro.WatiN.Test.FirstWatInTest.InvoegenPersonen() in
E:\\meersko\\WhatIn\\Chiro.WatiN\\FirsWatInTest.cs: line 102

-- Oplossing:
**Explorer eens afsluiten of logout/login/reboot?**

```

WatiN Test Recorder
-------------------

Nog niet echt deftig aan de praat gekregen, het crasht regelmatig.
Waarschijnlijk doordat we een beta-versie gebruiken.
