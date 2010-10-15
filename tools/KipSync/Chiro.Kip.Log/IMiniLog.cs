namespace Chiro.Kip.Log
{
	/// <summary>
	/// Zeer minimale logging, in afwachting van iets beters. (TODO #307)
	/// </summary>
	public interface IMiniLog
	{
		/// <summary>
		/// Logt een bericht mbt de groep met id <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">(Kipdorp)ID van groep waarop logbericht van toepassing</param>
		/// <param name="bericht">Te loggen bericht</param>
		void BerichtLoggen(int groepID, string bericht);

		/// <summary>
		/// Logt een foutbericht mbt de groep met id <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">(Kipdorp)ID van groep waarop logbericht van toepassing</param>
		/// <param name="bericht">Te loggen foutbericht</param>
		void FoutLoggen(int groepID, string bericht);
	
	}
}