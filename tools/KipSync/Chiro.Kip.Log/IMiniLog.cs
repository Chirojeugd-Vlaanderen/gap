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
		void Log(int groepID, string bericht);
	}
}