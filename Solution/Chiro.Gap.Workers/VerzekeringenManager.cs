using System;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Manager voor alwat met verzekeringen te maken heeft.
	/// TODO: Dit was misschien beter een 'PersoonsVerzekeringenManager' geweest?
	/// </summary>
	public class VerzekeringenManager
	{
		private IDao<VerzekeringsType> _verzekeringenDao;

		/// <summary>
		/// Construeert een nieuwe verzekeringenmanager
		/// </summary>
		/// <param name="vdao">Data Access Object voor verzekeringstypes</param>
		public VerzekeringenManager(IDao<VerzekeringsType> vdao)
		{
			_verzekeringenDao = vdao;
		}


		/// <summary>
		/// Haalt een verzekeringstype op uit de database
		/// </summary>
		/// <param name="verzekering"></param>
		/// <returns></returns>
		public VerzekeringsType Ophalen(Verzekering verzekering)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Verzekert een lid
		/// </summary>
		/// <param name="l">Te verzekeren lid</param>
		/// <param name="verz">Type van de verzekering</param>
		/// <param name="beginDatum">Begindatum van de verzekering; moet in de toekomst liggen.</param>
		/// <param name="eindDatum">Einddatum van de verzekering</param>
		public void Verzekeren(Lid l, VerzekeringsType verz, DateTime beginDatum, DateTime eindDatum)
		{
			throw new NotImplementedException();
		}
	}
}
