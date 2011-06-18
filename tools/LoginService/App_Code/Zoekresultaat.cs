// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2009-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Configuration;
using System.DirectoryServices;

/// <summary>
/// Klasse die als schil dient rond een zoekresultaat in Active Directory,
/// om toe te laten dat er bepaalde acties op uitgevoerd worden
/// </summary>
public class Zoekresultaat
{
    /// <summary>
    /// Het eigenlijke zoekresultaat
    /// </summary>
    public DirectoryEntry UniekResultaat { get; private set; }

    public SearchResultCollection Resultaten { get; set; }

    /// <summary>
    /// Instantieert een zoekresultaatschil
    /// </summary>
    /// <param name="ldapRoot">Het domein waarin de account of groep zich bevindt, 
    /// eventueel de OU waarin we willen zoeken</param>
    /// <param name="filter">De zoekfilter die toegepast moet worden</param>
    public Zoekresultaat(string ldapRoot, String filter)
    {
        var entry = new DirectoryEntry(ldapRoot);
        entry.AuthenticationType = AuthenticationTypes.Secure;

        var zoeker = new DirectorySearcher(entry);
        zoeker.Filter = filter;

        Resultaten = zoeker.FindAll();
        if (Resultaten.Count > 0)
        {
            UniekResultaat = Resultaten[0].GetDirectoryEntry();
        }
    }
}