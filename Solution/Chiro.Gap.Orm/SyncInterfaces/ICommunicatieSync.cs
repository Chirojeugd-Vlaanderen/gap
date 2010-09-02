namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public interface ICommunicatieSync
	{
		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		void Verwijderen(CommunicatieVorm communicatieVorm);

		/// <summary>
		/// Bewaart een communicatievorm in Kipadmin
		/// </summary>
		/// <param name="commvorm">Te bewaren communicatievorm, gekoppeld aan persoon</param>
		void Toevoegen(CommunicatieVorm commvorm);
	}
}