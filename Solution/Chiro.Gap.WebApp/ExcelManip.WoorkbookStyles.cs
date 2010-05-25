using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Methods voor manipulatie Excelstreams
	/// </summary>
	/// <remarks>In deze partial class zit de method om het style-gedoe in orde te krijgen voor datums</remarks>
	public partial class ExcelManip
	{
		/// <summary>
		/// Dit blok lelijke code is overgenomen uit een bestaand Exceldocument.
		/// Het enige wat hier in moet gebeuren, is een style definieren voor datumweergave.
		/// (De stijl met volgnummer 1).  De rest is bloat, en mag weg.
		/// (Alleen is het niet gemakkelijk uit te vissen wat er precies moet blijven staan.
		/// Tsss... Naar 't schijnt is dit met ODF veel gemakkelijker :-P.)
		/// </summary>
		/// <param name="workbookStylesPart">Het 'workbooksStylePart' waarvoor de inhoud gemaakt
		/// moet worden :-/ </param>
		private void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart)
		{
			Stylesheet stylesheet1 = new Stylesheet();

			Fonts fonts1 = new Fonts() { Count = (UInt32Value)19U };

			Font font1 = new Font();
			FontSize fontSize1 = new FontSize() { Val = 11D };
			Color color1 = new Color() { Theme = (UInt32Value)1U };
			FontName fontName1 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

			font1.Append(fontSize1);
			font1.Append(color1);
			font1.Append(fontName1);
			font1.Append(fontFamilyNumbering1);
			font1.Append(fontScheme1);

			Font font2 = new Font();
			FontSize fontSize2 = new FontSize() { Val = 11D };
			Color color2 = new Color() { Indexed = (UInt32Value)8U };
			FontName fontName2 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };

			font2.Append(fontSize2);
			font2.Append(color2);
			font2.Append(fontName2);
			font2.Append(fontFamilyNumbering2);

			Font font3 = new Font();
			FontSize fontSize3 = new FontSize() { Val = 11D };
			Color color3 = new Color() { Theme = (UInt32Value)1U };
			FontName fontName3 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering3 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

			font3.Append(fontSize3);
			font3.Append(color3);
			font3.Append(fontName3);
			font3.Append(fontFamilyNumbering3);
			font3.Append(fontScheme2);

			Font font4 = new Font();
			FontSize fontSize4 = new FontSize() { Val = 11D };
			Color color4 = new Color() { Theme = (UInt32Value)0U };
			FontName fontName4 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering4 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme3 = new FontScheme() { Val = FontSchemeValues.Minor };

			font4.Append(fontSize4);
			font4.Append(color4);
			font4.Append(fontName4);
			font4.Append(fontFamilyNumbering4);
			font4.Append(fontScheme3);

			Font font5 = new Font();
			FontSize fontSize5 = new FontSize() { Val = 11D };
			Color color5 = new Color() { Rgb = "FF9C0006" };
			FontName fontName5 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering5 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme4 = new FontScheme() { Val = FontSchemeValues.Minor };

			font5.Append(fontSize5);
			font5.Append(color5);
			font5.Append(fontName5);
			font5.Append(fontFamilyNumbering5);
			font5.Append(fontScheme4);

			Font font6 = new Font();
			Bold bold1 = new Bold();
			FontSize fontSize6 = new FontSize() { Val = 11D };
			Color color6 = new Color() { Rgb = "FFFA7D00" };
			FontName fontName6 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering6 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme5 = new FontScheme() { Val = FontSchemeValues.Minor };

			font6.Append(bold1);
			font6.Append(fontSize6);
			font6.Append(color6);
			font6.Append(fontName6);
			font6.Append(fontFamilyNumbering6);
			font6.Append(fontScheme5);

			Font font7 = new Font();
			Bold bold2 = new Bold();
			FontSize fontSize7 = new FontSize() { Val = 11D };
			Color color7 = new Color() { Theme = (UInt32Value)0U };
			FontName fontName7 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering7 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme6 = new FontScheme() { Val = FontSchemeValues.Minor };

			font7.Append(bold2);
			font7.Append(fontSize7);
			font7.Append(color7);
			font7.Append(fontName7);
			font7.Append(fontFamilyNumbering7);
			font7.Append(fontScheme6);

			Font font8 = new Font();
			Italic italic1 = new Italic();
			FontSize fontSize8 = new FontSize() { Val = 11D };
			Color color8 = new Color() { Rgb = "FF7F7F7F" };
			FontName fontName8 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering8 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme7 = new FontScheme() { Val = FontSchemeValues.Minor };

			font8.Append(italic1);
			font8.Append(fontSize8);
			font8.Append(color8);
			font8.Append(fontName8);
			font8.Append(fontFamilyNumbering8);
			font8.Append(fontScheme7);

			Font font9 = new Font();
			FontSize fontSize9 = new FontSize() { Val = 11D };
			Color color9 = new Color() { Rgb = "FF006100" };
			FontName fontName9 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering9 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme8 = new FontScheme() { Val = FontSchemeValues.Minor };

			font9.Append(fontSize9);
			font9.Append(color9);
			font9.Append(fontName9);
			font9.Append(fontFamilyNumbering9);
			font9.Append(fontScheme8);

			Font font10 = new Font();
			Bold bold3 = new Bold();
			FontSize fontSize10 = new FontSize() { Val = 15D };
			Color color10 = new Color() { Theme = (UInt32Value)3U };
			FontName fontName10 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering10 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme9 = new FontScheme() { Val = FontSchemeValues.Minor };

			font10.Append(bold3);
			font10.Append(fontSize10);
			font10.Append(color10);
			font10.Append(fontName10);
			font10.Append(fontFamilyNumbering10);
			font10.Append(fontScheme9);

			Font font11 = new Font();
			Bold bold4 = new Bold();
			FontSize fontSize11 = new FontSize() { Val = 13D };
			Color color11 = new Color() { Theme = (UInt32Value)3U };
			FontName fontName11 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering11 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme10 = new FontScheme() { Val = FontSchemeValues.Minor };

			font11.Append(bold4);
			font11.Append(fontSize11);
			font11.Append(color11);
			font11.Append(fontName11);
			font11.Append(fontFamilyNumbering11);
			font11.Append(fontScheme10);

			Font font12 = new Font();
			Bold bold5 = new Bold();
			FontSize fontSize12 = new FontSize() { Val = 11D };
			Color color12 = new Color() { Theme = (UInt32Value)3U };
			FontName fontName12 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering12 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme11 = new FontScheme() { Val = FontSchemeValues.Minor };

			font12.Append(bold5);
			font12.Append(fontSize12);
			font12.Append(color12);
			font12.Append(fontName12);
			font12.Append(fontFamilyNumbering12);
			font12.Append(fontScheme11);

			Font font13 = new Font();
			FontSize fontSize13 = new FontSize() { Val = 11D };
			Color color13 = new Color() { Rgb = "FF3F3F76" };
			FontName fontName13 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering13 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme12 = new FontScheme() { Val = FontSchemeValues.Minor };

			font13.Append(fontSize13);
			font13.Append(color13);
			font13.Append(fontName13);
			font13.Append(fontFamilyNumbering13);
			font13.Append(fontScheme12);

			Font font14 = new Font();
			FontSize fontSize14 = new FontSize() { Val = 11D };
			Color color14 = new Color() { Rgb = "FFFA7D00" };
			FontName fontName14 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering14 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme13 = new FontScheme() { Val = FontSchemeValues.Minor };

			font14.Append(fontSize14);
			font14.Append(color14);
			font14.Append(fontName14);
			font14.Append(fontFamilyNumbering14);
			font14.Append(fontScheme13);

			Font font15 = new Font();
			FontSize fontSize15 = new FontSize() { Val = 11D };
			Color color15 = new Color() { Rgb = "FF9C6500" };
			FontName fontName15 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering15 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme14 = new FontScheme() { Val = FontSchemeValues.Minor };

			font15.Append(fontSize15);
			font15.Append(color15);
			font15.Append(fontName15);
			font15.Append(fontFamilyNumbering15);
			font15.Append(fontScheme14);

			Font font16 = new Font();
			Bold bold6 = new Bold();
			FontSize fontSize16 = new FontSize() { Val = 11D };
			Color color16 = new Color() { Rgb = "FF3F3F3F" };
			FontName fontName16 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering16 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme15 = new FontScheme() { Val = FontSchemeValues.Minor };

			font16.Append(bold6);
			font16.Append(fontSize16);
			font16.Append(color16);
			font16.Append(fontName16);
			font16.Append(fontFamilyNumbering16);
			font16.Append(fontScheme15);

			Font font17 = new Font();
			Bold bold7 = new Bold();
			FontSize fontSize17 = new FontSize() { Val = 18D };
			Color color17 = new Color() { Theme = (UInt32Value)3U };
			FontName fontName17 = new FontName() { Val = "Cambria" };
			FontFamilyNumbering fontFamilyNumbering17 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme16 = new FontScheme() { Val = FontSchemeValues.Major };

			font17.Append(bold7);
			font17.Append(fontSize17);
			font17.Append(color17);
			font17.Append(fontName17);
			font17.Append(fontFamilyNumbering17);
			font17.Append(fontScheme16);

			Font font18 = new Font();
			Bold bold8 = new Bold();
			FontSize fontSize18 = new FontSize() { Val = 11D };
			Color color18 = new Color() { Theme = (UInt32Value)1U };
			FontName fontName18 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering18 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme17 = new FontScheme() { Val = FontSchemeValues.Minor };

			font18.Append(bold8);
			font18.Append(fontSize18);
			font18.Append(color18);
			font18.Append(fontName18);
			font18.Append(fontFamilyNumbering18);
			font18.Append(fontScheme17);

			Font font19 = new Font();
			FontSize fontSize19 = new FontSize() { Val = 11D };
			Color color19 = new Color() { Rgb = "FFFF0000" };
			FontName fontName19 = new FontName() { Val = "Calibri" };
			FontFamilyNumbering fontFamilyNumbering19 = new FontFamilyNumbering() { Val = 2 };
			FontScheme fontScheme18 = new FontScheme() { Val = FontSchemeValues.Minor };

			font19.Append(fontSize19);
			font19.Append(color19);
			font19.Append(fontName19);
			font19.Append(fontFamilyNumbering19);
			font19.Append(fontScheme18);

			fonts1.Append(font1);
			fonts1.Append(font2);
			fonts1.Append(font3);
			fonts1.Append(font4);
			fonts1.Append(font5);
			fonts1.Append(font6);
			fonts1.Append(font7);
			fonts1.Append(font8);
			fonts1.Append(font9);
			fonts1.Append(font10);
			fonts1.Append(font11);
			fonts1.Append(font12);
			fonts1.Append(font13);
			fonts1.Append(font14);
			fonts1.Append(font15);
			fonts1.Append(font16);
			fonts1.Append(font17);
			fonts1.Append(font18);
			fonts1.Append(font19);

			Fills fills1 = new Fills() { Count = (UInt32Value)33U };

			Fill fill1 = new Fill();
			PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			Fill fill2 = new Fill();
			PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			Fill fill3 = new Fill();

			PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor1 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill3.Append(foregroundColor1);
			patternFill3.Append(backgroundColor1);

			fill3.Append(patternFill3);

			Fill fill4 = new Fill();

			PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor2 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill4.Append(foregroundColor2);
			patternFill4.Append(backgroundColor2);

			fill4.Append(patternFill4);

			Fill fill5 = new Fill();

			PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor3 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill5.Append(foregroundColor3);
			patternFill5.Append(backgroundColor3);

			fill5.Append(patternFill5);

			Fill fill6 = new Fill();

			PatternFill patternFill6 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor4 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor4 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill6.Append(foregroundColor4);
			patternFill6.Append(backgroundColor4);

			fill6.Append(patternFill6);

			Fill fill7 = new Fill();

			PatternFill patternFill7 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor5 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor5 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill7.Append(foregroundColor5);
			patternFill7.Append(backgroundColor5);

			fill7.Append(patternFill7);

			Fill fill8 = new Fill();

			PatternFill patternFill8 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor6 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.79998168889431442D };
			BackgroundColor backgroundColor6 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill8.Append(foregroundColor6);
			patternFill8.Append(backgroundColor6);

			fill8.Append(patternFill8);

			Fill fill9 = new Fill();

			PatternFill patternFill9 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor7 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor7 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill9.Append(foregroundColor7);
			patternFill9.Append(backgroundColor7);

			fill9.Append(patternFill9);

			Fill fill10 = new Fill();

			PatternFill patternFill10 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor8 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor8 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill10.Append(foregroundColor8);
			patternFill10.Append(backgroundColor8);

			fill10.Append(patternFill10);

			Fill fill11 = new Fill();

			PatternFill patternFill11 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor9 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor9 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill11.Append(foregroundColor9);
			patternFill11.Append(backgroundColor9);

			fill11.Append(patternFill11);

			Fill fill12 = new Fill();

			PatternFill patternFill12 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor10 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor10 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill12.Append(foregroundColor10);
			patternFill12.Append(backgroundColor10);

			fill12.Append(patternFill12);

			Fill fill13 = new Fill();

			PatternFill patternFill13 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor11 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor11 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill13.Append(foregroundColor11);
			patternFill13.Append(backgroundColor11);

			fill13.Append(patternFill13);

			Fill fill14 = new Fill();

			PatternFill patternFill14 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor12 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.59999389629810485D };
			BackgroundColor backgroundColor12 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill14.Append(foregroundColor12);
			patternFill14.Append(backgroundColor12);

			fill14.Append(patternFill14);

			Fill fill15 = new Fill();

			PatternFill patternFill15 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor13 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor13 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill15.Append(foregroundColor13);
			patternFill15.Append(backgroundColor13);

			fill15.Append(patternFill15);

			Fill fill16 = new Fill();

			PatternFill patternFill16 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor14 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor14 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill16.Append(foregroundColor14);
			patternFill16.Append(backgroundColor14);

			fill16.Append(patternFill16);

			Fill fill17 = new Fill();

			PatternFill patternFill17 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor15 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor15 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill17.Append(foregroundColor15);
			patternFill17.Append(backgroundColor15);

			fill17.Append(patternFill17);

			Fill fill18 = new Fill();

			PatternFill patternFill18 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor16 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor16 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill18.Append(foregroundColor16);
			patternFill18.Append(backgroundColor16);

			fill18.Append(patternFill18);

			Fill fill19 = new Fill();

			PatternFill patternFill19 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor17 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor17 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill19.Append(foregroundColor17);
			patternFill19.Append(backgroundColor17);

			fill19.Append(patternFill19);

			Fill fill20 = new Fill();

			PatternFill patternFill20 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor18 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.39997558519241921D };
			BackgroundColor backgroundColor18 = new BackgroundColor() { Indexed = (UInt32Value)65U };

			patternFill20.Append(foregroundColor18);
			patternFill20.Append(backgroundColor18);

			fill20.Append(patternFill20);

			Fill fill21 = new Fill();

			PatternFill patternFill21 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor19 = new ForegroundColor() { Theme = (UInt32Value)4U };

			patternFill21.Append(foregroundColor19);

			fill21.Append(patternFill21);

			Fill fill22 = new Fill();

			PatternFill patternFill22 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor20 = new ForegroundColor() { Theme = (UInt32Value)5U };

			patternFill22.Append(foregroundColor20);

			fill22.Append(patternFill22);

			Fill fill23 = new Fill();

			PatternFill patternFill23 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor21 = new ForegroundColor() { Theme = (UInt32Value)6U };

			patternFill23.Append(foregroundColor21);

			fill23.Append(patternFill23);

			Fill fill24 = new Fill();

			PatternFill patternFill24 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor22 = new ForegroundColor() { Theme = (UInt32Value)7U };

			patternFill24.Append(foregroundColor22);

			fill24.Append(patternFill24);

			Fill fill25 = new Fill();

			PatternFill patternFill25 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor23 = new ForegroundColor() { Theme = (UInt32Value)8U };

			patternFill25.Append(foregroundColor23);

			fill25.Append(patternFill25);

			Fill fill26 = new Fill();

			PatternFill patternFill26 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor24 = new ForegroundColor() { Theme = (UInt32Value)9U };

			patternFill26.Append(foregroundColor24);

			fill26.Append(patternFill26);

			Fill fill27 = new Fill();

			PatternFill patternFill27 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor25 = new ForegroundColor() { Rgb = "FFFFC7CE" };

			patternFill27.Append(foregroundColor25);

			fill27.Append(patternFill27);

			Fill fill28 = new Fill();

			PatternFill patternFill28 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor26 = new ForegroundColor() { Rgb = "FFF2F2F2" };

			patternFill28.Append(foregroundColor26);

			fill28.Append(patternFill28);

			Fill fill29 = new Fill();

			PatternFill patternFill29 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor27 = new ForegroundColor() { Rgb = "FFA5A5A5" };

			patternFill29.Append(foregroundColor27);

			fill29.Append(patternFill29);

			Fill fill30 = new Fill();

			PatternFill patternFill30 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor28 = new ForegroundColor() { Rgb = "FFC6EFCE" };

			patternFill30.Append(foregroundColor28);

			fill30.Append(patternFill30);

			Fill fill31 = new Fill();

			PatternFill patternFill31 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor29 = new ForegroundColor() { Rgb = "FFFFCC99" };

			patternFill31.Append(foregroundColor29);

			fill31.Append(patternFill31);

			Fill fill32 = new Fill();

			PatternFill patternFill32 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor30 = new ForegroundColor() { Rgb = "FFFFEB9C" };

			patternFill32.Append(foregroundColor30);

			fill32.Append(patternFill32);

			Fill fill33 = new Fill();

			PatternFill patternFill33 = new PatternFill() { PatternType = PatternValues.Solid };
			ForegroundColor foregroundColor31 = new ForegroundColor() { Rgb = "FFFFFFCC" };

			patternFill33.Append(foregroundColor31);

			fill33.Append(patternFill33);

			fills1.Append(fill1);
			fills1.Append(fill2);
			fills1.Append(fill3);
			fills1.Append(fill4);
			fills1.Append(fill5);
			fills1.Append(fill6);
			fills1.Append(fill7);
			fills1.Append(fill8);
			fills1.Append(fill9);
			fills1.Append(fill10);
			fills1.Append(fill11);
			fills1.Append(fill12);
			fills1.Append(fill13);
			fills1.Append(fill14);
			fills1.Append(fill15);
			fills1.Append(fill16);
			fills1.Append(fill17);
			fills1.Append(fill18);
			fills1.Append(fill19);
			fills1.Append(fill20);
			fills1.Append(fill21);
			fills1.Append(fill22);
			fills1.Append(fill23);
			fills1.Append(fill24);
			fills1.Append(fill25);
			fills1.Append(fill26);
			fills1.Append(fill27);
			fills1.Append(fill28);
			fills1.Append(fill29);
			fills1.Append(fill30);
			fills1.Append(fill31);
			fills1.Append(fill32);
			fills1.Append(fill33);

			Borders borders1 = new Borders() { Count = (UInt32Value)10U };

			Border border1 = new Border();
			LeftBorder leftBorder1 = new LeftBorder();
			RightBorder rightBorder1 = new RightBorder();
			TopBorder topBorder1 = new TopBorder();
			BottomBorder bottomBorder1 = new BottomBorder();
			DiagonalBorder diagonalBorder1 = new DiagonalBorder();

			border1.Append(leftBorder1);
			border1.Append(rightBorder1);
			border1.Append(topBorder1);
			border1.Append(bottomBorder1);
			border1.Append(diagonalBorder1);

			Border border2 = new Border();

			LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
			Color color20 = new Color() { Rgb = "FF7F7F7F" };

			leftBorder2.Append(color20);

			RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
			Color color21 = new Color() { Rgb = "FF7F7F7F" };

			rightBorder2.Append(color21);

			TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
			Color color22 = new Color() { Rgb = "FF7F7F7F" };

			topBorder2.Append(color22);

			BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
			Color color23 = new Color() { Rgb = "FF7F7F7F" };

			bottomBorder2.Append(color23);
			DiagonalBorder diagonalBorder2 = new DiagonalBorder();

			border2.Append(leftBorder2);
			border2.Append(rightBorder2);
			border2.Append(topBorder2);
			border2.Append(bottomBorder2);
			border2.Append(diagonalBorder2);

			Border border3 = new Border();

			LeftBorder leftBorder3 = new LeftBorder() { Style = BorderStyleValues.Double };
			Color color24 = new Color() { Rgb = "FF3F3F3F" };

			leftBorder3.Append(color24);

			RightBorder rightBorder3 = new RightBorder() { Style = BorderStyleValues.Double };
			Color color25 = new Color() { Rgb = "FF3F3F3F" };

			rightBorder3.Append(color25);

			TopBorder topBorder3 = new TopBorder() { Style = BorderStyleValues.Double };
			Color color26 = new Color() { Rgb = "FF3F3F3F" };

			topBorder3.Append(color26);

			BottomBorder bottomBorder3 = new BottomBorder() { Style = BorderStyleValues.Double };
			Color color27 = new Color() { Rgb = "FF3F3F3F" };

			bottomBorder3.Append(color27);
			DiagonalBorder diagonalBorder3 = new DiagonalBorder();

			border3.Append(leftBorder3);
			border3.Append(rightBorder3);
			border3.Append(topBorder3);
			border3.Append(bottomBorder3);
			border3.Append(diagonalBorder3);

			Border border4 = new Border();
			LeftBorder leftBorder4 = new LeftBorder();
			RightBorder rightBorder4 = new RightBorder();
			TopBorder topBorder4 = new TopBorder();

			BottomBorder bottomBorder4 = new BottomBorder() { Style = BorderStyleValues.Thick };
			Color color28 = new Color() { Theme = (UInt32Value)4U };

			bottomBorder4.Append(color28);
			DiagonalBorder diagonalBorder4 = new DiagonalBorder();

			border4.Append(leftBorder4);
			border4.Append(rightBorder4);
			border4.Append(topBorder4);
			border4.Append(bottomBorder4);
			border4.Append(diagonalBorder4);

			Border border5 = new Border();
			LeftBorder leftBorder5 = new LeftBorder();
			RightBorder rightBorder5 = new RightBorder();
			TopBorder topBorder5 = new TopBorder();

			BottomBorder bottomBorder5 = new BottomBorder() { Style = BorderStyleValues.Thick };
			Color color29 = new Color() { Theme = (UInt32Value)4U, Tint = 0.499984740745262D };

			bottomBorder5.Append(color29);
			DiagonalBorder diagonalBorder5 = new DiagonalBorder();

			border5.Append(leftBorder5);
			border5.Append(rightBorder5);
			border5.Append(topBorder5);
			border5.Append(bottomBorder5);
			border5.Append(diagonalBorder5);

			Border border6 = new Border();
			LeftBorder leftBorder6 = new LeftBorder();
			RightBorder rightBorder6 = new RightBorder();
			TopBorder topBorder6 = new TopBorder();

			BottomBorder bottomBorder6 = new BottomBorder() { Style = BorderStyleValues.Medium };
			Color color30 = new Color() { Theme = (UInt32Value)4U, Tint = 0.39997558519241921D };

			bottomBorder6.Append(color30);
			DiagonalBorder diagonalBorder6 = new DiagonalBorder();

			border6.Append(leftBorder6);
			border6.Append(rightBorder6);
			border6.Append(topBorder6);
			border6.Append(bottomBorder6);
			border6.Append(diagonalBorder6);

			Border border7 = new Border();
			LeftBorder leftBorder7 = new LeftBorder();
			RightBorder rightBorder7 = new RightBorder();
			TopBorder topBorder7 = new TopBorder();

			BottomBorder bottomBorder7 = new BottomBorder() { Style = BorderStyleValues.Double };
			Color color31 = new Color() { Rgb = "FFFF8001" };

			bottomBorder7.Append(color31);
			DiagonalBorder diagonalBorder7 = new DiagonalBorder();

			border7.Append(leftBorder7);
			border7.Append(rightBorder7);
			border7.Append(topBorder7);
			border7.Append(bottomBorder7);
			border7.Append(diagonalBorder7);

			Border border8 = new Border();

			LeftBorder leftBorder8 = new LeftBorder() { Style = BorderStyleValues.Thin };
			Color color32 = new Color() { Rgb = "FFB2B2B2" };

			leftBorder8.Append(color32);

			RightBorder rightBorder8 = new RightBorder() { Style = BorderStyleValues.Thin };
			Color color33 = new Color() { Rgb = "FFB2B2B2" };

			rightBorder8.Append(color33);

			TopBorder topBorder8 = new TopBorder() { Style = BorderStyleValues.Thin };
			Color color34 = new Color() { Rgb = "FFB2B2B2" };

			topBorder8.Append(color34);

			BottomBorder bottomBorder8 = new BottomBorder() { Style = BorderStyleValues.Thin };
			Color color35 = new Color() { Rgb = "FFB2B2B2" };

			bottomBorder8.Append(color35);
			DiagonalBorder diagonalBorder8 = new DiagonalBorder();

			border8.Append(leftBorder8);
			border8.Append(rightBorder8);
			border8.Append(topBorder8);
			border8.Append(bottomBorder8);
			border8.Append(diagonalBorder8);

			Border border9 = new Border();

			LeftBorder leftBorder9 = new LeftBorder() { Style = BorderStyleValues.Thin };
			Color color36 = new Color() { Rgb = "FF3F3F3F" };

			leftBorder9.Append(color36);

			RightBorder rightBorder9 = new RightBorder() { Style = BorderStyleValues.Thin };
			Color color37 = new Color() { Rgb = "FF3F3F3F" };

			rightBorder9.Append(color37);

			TopBorder topBorder9 = new TopBorder() { Style = BorderStyleValues.Thin };
			Color color38 = new Color() { Rgb = "FF3F3F3F" };

			topBorder9.Append(color38);

			BottomBorder bottomBorder9 = new BottomBorder() { Style = BorderStyleValues.Thin };
			Color color39 = new Color() { Rgb = "FF3F3F3F" };

			bottomBorder9.Append(color39);
			DiagonalBorder diagonalBorder9 = new DiagonalBorder();

			border9.Append(leftBorder9);
			border9.Append(rightBorder9);
			border9.Append(topBorder9);
			border9.Append(bottomBorder9);
			border9.Append(diagonalBorder9);

			Border border10 = new Border();
			LeftBorder leftBorder10 = new LeftBorder();
			RightBorder rightBorder10 = new RightBorder();

			TopBorder topBorder10 = new TopBorder() { Style = BorderStyleValues.Thin };
			Color color40 = new Color() { Theme = (UInt32Value)4U };

			topBorder10.Append(color40);

			BottomBorder bottomBorder10 = new BottomBorder() { Style = BorderStyleValues.Double };
			Color color41 = new Color() { Theme = (UInt32Value)4U };

			bottomBorder10.Append(color41);
			DiagonalBorder diagonalBorder10 = new DiagonalBorder();

			border10.Append(leftBorder10);
			border10.Append(rightBorder10);
			border10.Append(topBorder10);
			border10.Append(bottomBorder10);
			border10.Append(diagonalBorder10);

			borders1.Append(border1);
			borders1.Append(border2);
			borders1.Append(border3);
			borders1.Append(border4);
			borders1.Append(border5);
			borders1.Append(border6);
			borders1.Append(border7);
			borders1.Append(border8);
			borders1.Append(border9);
			borders1.Append(border10);

			CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)42U };
			CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
			CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)5U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)6U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat7 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)7U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat8 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)8U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat9 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)9U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat10 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)10U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat11 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)11U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat12 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)12U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat13 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)13U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat14 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)14U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat15 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)15U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat16 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)16U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat17 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)17U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat18 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)18U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat19 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)19U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat20 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)20U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat21 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)21U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat22 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)22U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat23 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)23U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat24 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)24U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat25 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)25U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat26 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)4U, FillId = (UInt32Value)26U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat27 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)5U, FillId = (UInt32Value)27U, BorderId = (UInt32Value)1U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat28 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)6U, FillId = (UInt32Value)28U, BorderId = (UInt32Value)2U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat29 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)7U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat30 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)8U, FillId = (UInt32Value)29U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat31 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)9U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)3U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat32 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)10U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)4U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat33 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)11U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)5U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat34 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)11U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat35 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)12U, FillId = (UInt32Value)30U, BorderId = (UInt32Value)1U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat36 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)13U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)6U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat37 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)14U, FillId = (UInt32Value)31U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat38 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)32U, BorderId = (UInt32Value)7U, ApplyNumberFormat = false, ApplyFont = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat39 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)15U, FillId = (UInt32Value)27U, BorderId = (UInt32Value)8U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat40 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)16U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat41 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)9U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			CellFormat cellFormat42 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)18U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };

			cellStyleFormats1.Append(cellFormat1);
			cellStyleFormats1.Append(cellFormat2);
			cellStyleFormats1.Append(cellFormat3);
			cellStyleFormats1.Append(cellFormat4);
			cellStyleFormats1.Append(cellFormat5);
			cellStyleFormats1.Append(cellFormat6);
			cellStyleFormats1.Append(cellFormat7);
			cellStyleFormats1.Append(cellFormat8);
			cellStyleFormats1.Append(cellFormat9);
			cellStyleFormats1.Append(cellFormat10);
			cellStyleFormats1.Append(cellFormat11);
			cellStyleFormats1.Append(cellFormat12);
			cellStyleFormats1.Append(cellFormat13);
			cellStyleFormats1.Append(cellFormat14);
			cellStyleFormats1.Append(cellFormat15);
			cellStyleFormats1.Append(cellFormat16);
			cellStyleFormats1.Append(cellFormat17);
			cellStyleFormats1.Append(cellFormat18);
			cellStyleFormats1.Append(cellFormat19);
			cellStyleFormats1.Append(cellFormat20);
			cellStyleFormats1.Append(cellFormat21);
			cellStyleFormats1.Append(cellFormat22);
			cellStyleFormats1.Append(cellFormat23);
			cellStyleFormats1.Append(cellFormat24);
			cellStyleFormats1.Append(cellFormat25);
			cellStyleFormats1.Append(cellFormat26);
			cellStyleFormats1.Append(cellFormat27);
			cellStyleFormats1.Append(cellFormat28);
			cellStyleFormats1.Append(cellFormat29);
			cellStyleFormats1.Append(cellFormat30);
			cellStyleFormats1.Append(cellFormat31);
			cellStyleFormats1.Append(cellFormat32);
			cellStyleFormats1.Append(cellFormat33);
			cellStyleFormats1.Append(cellFormat34);
			cellStyleFormats1.Append(cellFormat35);
			cellStyleFormats1.Append(cellFormat36);
			cellStyleFormats1.Append(cellFormat37);
			cellStyleFormats1.Append(cellFormat38);
			cellStyleFormats1.Append(cellFormat39);
			cellStyleFormats1.Append(cellFormat40);
			cellStyleFormats1.Append(cellFormat41);
			cellStyleFormats1.Append(cellFormat42);

			CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)2U };
			CellFormat cellFormat43 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
			CellFormat cellFormat44 = new CellFormat() { NumberFormatId = (UInt32Value)14U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true };

			cellFormats1.Append(cellFormat43);
			cellFormats1.Append(cellFormat44);

			CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)42U };
			CellStyle cellStyle1 = new CellStyle() { Name = "20% - Accent1", FormatId = (UInt32Value)1U, BuiltinId = (UInt32Value)30U, CustomBuiltin = true };
			CellStyle cellStyle2 = new CellStyle() { Name = "20% - Accent2", FormatId = (UInt32Value)2U, BuiltinId = (UInt32Value)34U, CustomBuiltin = true };
			CellStyle cellStyle3 = new CellStyle() { Name = "20% - Accent3", FormatId = (UInt32Value)3U, BuiltinId = (UInt32Value)38U, CustomBuiltin = true };
			CellStyle cellStyle4 = new CellStyle() { Name = "20% - Accent4", FormatId = (UInt32Value)4U, BuiltinId = (UInt32Value)42U, CustomBuiltin = true };
			CellStyle cellStyle5 = new CellStyle() { Name = "20% - Accent5", FormatId = (UInt32Value)5U, BuiltinId = (UInt32Value)46U, CustomBuiltin = true };
			CellStyle cellStyle6 = new CellStyle() { Name = "20% - Accent6", FormatId = (UInt32Value)6U, BuiltinId = (UInt32Value)50U, CustomBuiltin = true };
			CellStyle cellStyle7 = new CellStyle() { Name = "40% - Accent1", FormatId = (UInt32Value)7U, BuiltinId = (UInt32Value)31U, CustomBuiltin = true };
			CellStyle cellStyle8 = new CellStyle() { Name = "40% - Accent2", FormatId = (UInt32Value)8U, BuiltinId = (UInt32Value)35U, CustomBuiltin = true };
			CellStyle cellStyle9 = new CellStyle() { Name = "40% - Accent3", FormatId = (UInt32Value)9U, BuiltinId = (UInt32Value)39U, CustomBuiltin = true };
			CellStyle cellStyle10 = new CellStyle() { Name = "40% - Accent4", FormatId = (UInt32Value)10U, BuiltinId = (UInt32Value)43U, CustomBuiltin = true };
			CellStyle cellStyle11 = new CellStyle() { Name = "40% - Accent5", FormatId = (UInt32Value)11U, BuiltinId = (UInt32Value)47U, CustomBuiltin = true };
			CellStyle cellStyle12 = new CellStyle() { Name = "40% - Accent6", FormatId = (UInt32Value)12U, BuiltinId = (UInt32Value)51U, CustomBuiltin = true };
			CellStyle cellStyle13 = new CellStyle() { Name = "60% - Accent1", FormatId = (UInt32Value)13U, BuiltinId = (UInt32Value)32U, CustomBuiltin = true };
			CellStyle cellStyle14 = new CellStyle() { Name = "60% - Accent2", FormatId = (UInt32Value)14U, BuiltinId = (UInt32Value)36U, CustomBuiltin = true };
			CellStyle cellStyle15 = new CellStyle() { Name = "60% - Accent3", FormatId = (UInt32Value)15U, BuiltinId = (UInt32Value)40U, CustomBuiltin = true };
			CellStyle cellStyle16 = new CellStyle() { Name = "60% - Accent4", FormatId = (UInt32Value)16U, BuiltinId = (UInt32Value)44U, CustomBuiltin = true };
			CellStyle cellStyle17 = new CellStyle() { Name = "60% - Accent5", FormatId = (UInt32Value)17U, BuiltinId = (UInt32Value)48U, CustomBuiltin = true };
			CellStyle cellStyle18 = new CellStyle() { Name = "60% - Accent6", FormatId = (UInt32Value)18U, BuiltinId = (UInt32Value)52U, CustomBuiltin = true };
			CellStyle cellStyle19 = new CellStyle() { Name = "Accent1", FormatId = (UInt32Value)19U, BuiltinId = (UInt32Value)29U, CustomBuiltin = true };
			CellStyle cellStyle20 = new CellStyle() { Name = "Accent2", FormatId = (UInt32Value)20U, BuiltinId = (UInt32Value)33U, CustomBuiltin = true };
			CellStyle cellStyle21 = new CellStyle() { Name = "Accent3", FormatId = (UInt32Value)21U, BuiltinId = (UInt32Value)37U, CustomBuiltin = true };
			CellStyle cellStyle22 = new CellStyle() { Name = "Accent4", FormatId = (UInt32Value)22U, BuiltinId = (UInt32Value)41U, CustomBuiltin = true };
			CellStyle cellStyle23 = new CellStyle() { Name = "Accent5", FormatId = (UInt32Value)23U, BuiltinId = (UInt32Value)45U, CustomBuiltin = true };
			CellStyle cellStyle24 = new CellStyle() { Name = "Accent6", FormatId = (UInt32Value)24U, BuiltinId = (UInt32Value)49U, CustomBuiltin = true };
			CellStyle cellStyle25 = new CellStyle() { Name = "Bad", FormatId = (UInt32Value)25U, BuiltinId = (UInt32Value)27U, CustomBuiltin = true };
			CellStyle cellStyle26 = new CellStyle() { Name = "Calculation", FormatId = (UInt32Value)26U, BuiltinId = (UInt32Value)22U, CustomBuiltin = true };
			CellStyle cellStyle27 = new CellStyle() { Name = "Check Cell", FormatId = (UInt32Value)27U, BuiltinId = (UInt32Value)23U, CustomBuiltin = true };
			CellStyle cellStyle28 = new CellStyle() { Name = "Explanatory Text", FormatId = (UInt32Value)28U, BuiltinId = (UInt32Value)53U, CustomBuiltin = true };
			CellStyle cellStyle29 = new CellStyle() { Name = "Good", FormatId = (UInt32Value)29U, BuiltinId = (UInt32Value)26U, CustomBuiltin = true };
			CellStyle cellStyle30 = new CellStyle() { Name = "Heading 1", FormatId = (UInt32Value)30U, BuiltinId = (UInt32Value)16U, CustomBuiltin = true };
			CellStyle cellStyle31 = new CellStyle() { Name = "Heading 2", FormatId = (UInt32Value)31U, BuiltinId = (UInt32Value)17U, CustomBuiltin = true };
			CellStyle cellStyle32 = new CellStyle() { Name = "Heading 3", FormatId = (UInt32Value)32U, BuiltinId = (UInt32Value)18U, CustomBuiltin = true };
			CellStyle cellStyle33 = new CellStyle() { Name = "Heading 4", FormatId = (UInt32Value)33U, BuiltinId = (UInt32Value)19U, CustomBuiltin = true };
			CellStyle cellStyle34 = new CellStyle() { Name = "Input", FormatId = (UInt32Value)34U, BuiltinId = (UInt32Value)20U, CustomBuiltin = true };
			CellStyle cellStyle35 = new CellStyle() { Name = "Linked Cell", FormatId = (UInt32Value)35U, BuiltinId = (UInt32Value)24U, CustomBuiltin = true };
			CellStyle cellStyle36 = new CellStyle() { Name = "Neutral", FormatId = (UInt32Value)36U, BuiltinId = (UInt32Value)28U, CustomBuiltin = true };
			CellStyle cellStyle37 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
			CellStyle cellStyle38 = new CellStyle() { Name = "Note", FormatId = (UInt32Value)37U, BuiltinId = (UInt32Value)10U, CustomBuiltin = true };
			CellStyle cellStyle39 = new CellStyle() { Name = "Output", FormatId = (UInt32Value)38U, BuiltinId = (UInt32Value)21U, CustomBuiltin = true };
			CellStyle cellStyle40 = new CellStyle() { Name = "Title", FormatId = (UInt32Value)39U, BuiltinId = (UInt32Value)15U, CustomBuiltin = true };
			CellStyle cellStyle41 = new CellStyle() { Name = "Total", FormatId = (UInt32Value)40U, BuiltinId = (UInt32Value)25U, CustomBuiltin = true };
			CellStyle cellStyle42 = new CellStyle() { Name = "Warning Text", FormatId = (UInt32Value)41U, BuiltinId = (UInt32Value)11U, CustomBuiltin = true };

			cellStyles1.Append(cellStyle1);
			cellStyles1.Append(cellStyle2);
			cellStyles1.Append(cellStyle3);
			cellStyles1.Append(cellStyle4);
			cellStyles1.Append(cellStyle5);
			cellStyles1.Append(cellStyle6);
			cellStyles1.Append(cellStyle7);
			cellStyles1.Append(cellStyle8);
			cellStyles1.Append(cellStyle9);
			cellStyles1.Append(cellStyle10);
			cellStyles1.Append(cellStyle11);
			cellStyles1.Append(cellStyle12);
			cellStyles1.Append(cellStyle13);
			cellStyles1.Append(cellStyle14);
			cellStyles1.Append(cellStyle15);
			cellStyles1.Append(cellStyle16);
			cellStyles1.Append(cellStyle17);
			cellStyles1.Append(cellStyle18);
			cellStyles1.Append(cellStyle19);
			cellStyles1.Append(cellStyle20);
			cellStyles1.Append(cellStyle21);
			cellStyles1.Append(cellStyle22);
			cellStyles1.Append(cellStyle23);
			cellStyles1.Append(cellStyle24);
			cellStyles1.Append(cellStyle25);
			cellStyles1.Append(cellStyle26);
			cellStyles1.Append(cellStyle27);
			cellStyles1.Append(cellStyle28);
			cellStyles1.Append(cellStyle29);
			cellStyles1.Append(cellStyle30);
			cellStyles1.Append(cellStyle31);
			cellStyles1.Append(cellStyle32);
			cellStyles1.Append(cellStyle33);
			cellStyles1.Append(cellStyle34);
			cellStyles1.Append(cellStyle35);
			cellStyles1.Append(cellStyle36);
			cellStyles1.Append(cellStyle37);
			cellStyles1.Append(cellStyle38);
			cellStyles1.Append(cellStyle39);
			cellStyles1.Append(cellStyle40);
			cellStyles1.Append(cellStyle41);
			cellStyles1.Append(cellStyle42);
			DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
			TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium9", DefaultPivotStyle = "PivotStyleLight16" };

			stylesheet1.Append(fonts1);
			stylesheet1.Append(fills1);
			stylesheet1.Append(borders1);
			stylesheet1.Append(cellStyleFormats1);
			stylesheet1.Append(cellFormats1);
			stylesheet1.Append(cellStyles1);
			stylesheet1.Append(differentialFormats1);
			stylesheet1.Append(tableStyles1);

			workbookStylesPart.Stylesheet = stylesheet1;
		}
	}
}
