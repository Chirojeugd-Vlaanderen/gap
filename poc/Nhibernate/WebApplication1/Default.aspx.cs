using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CgDal;

namespace WebApplication1
{

    // Deze webapplicatie laat toe om naam, voornaam, geboortedatum en
    // telefoonnummers van een persoon te wijzigen.
    //
    // Het persoonsobject wordt bewaard in een sessievariabele.  Alle
    // wijzigingen aan de controls op het webform worden in eerste 
    // instantie bewaard in de sessievariabele.
    //
    // Pas als de gebruiker op 'bewaren' klikt, worden de gegevens
    // naar de database gestuurd.

    public partial class _Default : System.Web.UI.Page
    {
        static readonly int testPersoonID = 1894;

        /// <summary>
        /// Toont de persoonsgegevens (uit de sessie) in het form
        /// </summary>
        /// <param name="teBewerkenRij">Als deze waarde verschilt van
        /// -1, dan geeft deze parameter aan welke rij in de telefoon-
        /// nummertabel 'editeerbaar' is.</param>
        private void ToonPersoonsInfo(int teBewerkenRij)
        {
            Persoon p = SessionStateUtility.GetoondePersoon;

            voorNaamTextBox.Text = p.VoorNaam;
            naamTextBox.Text = p.Naam;
            geboorteDatumCalendar.SelectedDate = p.GeboorteDatum.Value;
            geboorteDatumCalendar.VisibleDate = p.GeboorteDatum.Value;
            telefoonNrsGridView.DataSource = (from cv in p.CommunicatieVorms
                                              where cv.CommunicatieTypeID == 1 && !cv.TeVerwijderen
                                              select cv).ToList();
            telefoonNrsGridView.EditIndex = teBewerkenRij;
            telefoonNrsGridView.DataBind();

        }

        /// <summary>
        /// Als de pagina voor het eerst aangeroepen wordt (geen postback),
        /// dan wordt de personenservice aangesproken om de gegevens van
        /// de testpersoon op te halen, en in een sessievariabele te 
        /// bewaren.
        /// 
        /// De persoonsinformatie wordt in de controls op het form getoond.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /// Haal persoonsgegevens op

                using (PersonenServiceReference.PersonenServiceClient service = new WebApplication1.PersonenServiceReference.PersonenServiceClient())
                {
                    SessionStateUtility.GetoondePersoon = service.PersoonMetDetailsGet(testPersoonID);
                    ToonPersoonsInfo(-1);
                }
            }
        }

        /// <summary>
        /// Deze functie retourneert het communicatievormobject waaruit
        /// de gegevens van een rij in de tabel bepaalt.
        /// </summary>
        /// <param name="index">gevraagde tabelrij</param>
        /// <returns>communicatieovormobject</returns>
        private CommunicatieVorm CommunicatieVormUitTabel(int index)
        {
            int id = telefoonNrsGridView.DataKeys[index].Value as int? ?? 0;
            return (from cv in SessionStateUtility.GetoondePersoon.CommunicatieVorms
                    where cv.CommunicatieVormID == id
                    select cv).Single<CommunicatieVorm>();
        }

        /// <summary>
        /// Ingetikt telefoonnummer koppelen aan persoon in sessie
        /// </summary>
        private void TelefoonNummerToevoegen()
        {
            CommunicatieVorm cv = new CommunicatieVorm();
            cv.PersoonID = SessionStateUtility.GetoondePersoon.PersoonID;
            cv.Persoon = SessionStateUtility.GetoondePersoon;
            cv.CommunicatieTypeID = 1;
            cv.Nummer = nieuwTelefoonNrTextBox.Text;
            cv.CommunicatieVormID = 0;

            SessionStateUtility.GetoondePersoon.CommunicatieVorms.Add(cv);

            ToonPersoonsInfo(-1);
        }

        /// <summary>
        /// Verwijdert telefoonnummer op basis van index in tabel
        /// </summary>
        /// <param name="index">index te verwijderen nummer</param>
        private void TelefoonNummerVerwijderen(int index)
        {
            CommunicatieVorm cv = CommunicatieVormUitTabel(index);

            if (cv.CommunicatieVormID <= 0)
            {
                SessionStateUtility.GetoondePersoon.CommunicatieVorms.Remove(cv);
            }
            else
            {
                cv.TeVerwijderen = true;
            }

            ToonPersoonsInfo(-1);
        }

        /// <summary>
        /// Updatet telefoonnummer met gegeven index op basis van ingetikte
        /// gegevens in tabel.
        /// </summary>
        /// <param name="index">rij met te updaten gegevens</param>
        private void TelefoonNummerWijzigen(int index)
        {
            CommunicatieVorm cv = CommunicatieVormUitTabel(index);

            cv.Nummer = (telefoonNrsGridView.Rows[index].FindControl("TelefoonNrTextBox") as TextBox).Text;
            ToonPersoonsInfo(-1);
        }

        /// <summary>
        /// Persoonsgegevens in het sessie-object persisteren
        /// </summary>
        private void PersoonsGegevensBewaren()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebApplication1.PersonenServiceReference.PersonenServiceClient())
            {
                service.PersoonUpdaten(SessionStateUtility.GetoondePersoon);

                // Direct opnieuw ophalen, omdat je opnieuw in hetzelfde
                // form terecht komt.  Anders krijg je 'change conflicts'
                // als de wijzigingen nog eens gepost worden.

                SessionStateUtility.GetoondePersoon
                    = service.PersoonMetDetailsGet(testPersoonID);
            }
        }

        /// <summary>
        /// Haalt persoonsgegevens voor persoon in sessie opnieuw op via
        /// service, en updatet controls op webform.
        /// </summary>
        private void PersoonsGegevensHerstellen()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebApplication1.PersonenServiceReference.PersonenServiceClient())
            {
                SessionStateUtility.GetoondePersoon
                    = service.PersoonMetDetailsGet(testPersoonID);
                ToonPersoonsInfo(-1);
            }
        }


        #region events


        protected void telefoonNrToevoegenButton_Click(object sender, EventArgs e)
        {
            TelefoonNummerToevoegen();
        }

        protected void telefoonNrsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            TelefoonNummerVerwijderen(e.RowIndex);
        }

        protected void telefoonNrsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // 'bewerken' aangeklikt; refresh en maak geselecteerde rij 'editable'.
            ToonPersoonsInfo(e.NewEditIndex);
        }

        protected void telefoonNrsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TelefoonNummerWijzigen(e.RowIndex);
        }

        protected void telefoonNrsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // rijwijziging geannuleerd; toon gewoon persoonsobject in
            // session opnieuw.

            ToonPersoonsInfo(-1);
        }

        protected void bewarenKnop_Click(object sender, EventArgs e)
        {
            PersoonsGegevensBewaren();
        }

        protected void herstellenKnop_Click(object sender, EventArgs e)
        {
            PersoonsGegevensHerstellen();
        }

        protected void naamTextBox_TextChanged(object sender, EventArgs e)
        {
            SessionStateUtility.GetoondePersoon.Naam = naamTextBox.Text;
        }

        protected void voorNaamTextBox_TextChanged(object sender, EventArgs e)
        {
            SessionStateUtility.GetoondePersoon.VoorNaam = voorNaamTextBox.Text;
        }

        protected void geboorteDatumCalendar_SelectionChanged(object sender, EventArgs e)
        {
            SessionStateUtility.GetoondePersoon.GeboorteDatum = geboorteDatumCalendar.SelectedDate;
        }

        #endregion

    }
}
