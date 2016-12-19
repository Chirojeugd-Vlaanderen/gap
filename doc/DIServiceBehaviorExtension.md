Om dependency injection te gebruiken voor WCF-services, creerden we een
service behavior extension: DIServiceBehaviorExtension. Je gebruikt ze
als volgt:

**** Leg in je project een referentie naar
Chiro.Cdf.!DependencyInjectionBehavior

**** Voeg in Web.Config het volgende toe voor in system.serviceModel:

```
&lt;extensions&gt;
&lt;behaviorExtensions&gt;
&lt;add name="DIServiceBehaviorExtension"
type="Chiro.Cdf.DependencyInjection.DIServiceBehaviorSection,
Chiro.Cdf.DependencyInjectionBehavior, Version=1.0.0.0, Culture=neutral,
[PublicKeyToken](PublicKeyToken.md)=null"/&gt;
&lt;/behaviorExtensions&gt;
&lt;/extensions&gt;
```

**** In de definitie van de service behavior die je voor je service
gebruikt, voeg je dit toe:

```
&lt;DIServiceBehaviorExtension /&gt;
```
