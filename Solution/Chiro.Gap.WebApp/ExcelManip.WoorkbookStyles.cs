// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
		/// Definieert opmaak voor een Excel-bestand, onder andere voor de datum
		/// </summary>
		/// <param name="workbookStylesPart">Het 'workbooksStylePart' waarvoor de inhoud gemaakt
		/// moet worden :-/ </param>
		private void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart)
		{
			// Dit blok lelijke code is overgenomen uit een bestaand Exceldocument.
			// Het enige wat hier in moet gebeuren, is een style definieren voor datumweergave.
			// (De stijl met volgnummer 1).  De rest is bloat, en mag weg.
			// (Alleen is het niet gemakkelijk uit te vissen wat er precies moet blijven staan.
			// Tsss... Naar 't schijnt is dit met ODF veel gemakkelijker :-P.)

			var stylesheet1 = new Stylesheet();

			var fonts1 = new Fonts { Count = 19U };

			var font1 = new Font();
			var fontSize1 = new FontSize { Val = 11D };
			var color1 = new Color { Theme = 1U };
			var fontName1 = new FontName { Val = "Arial" };
			var fontFamilyNumbering1 = new FontFamilyNumbering { Val = 2 };
			var fontScheme1 = new FontScheme { Val = FontSchemeValues.Minor };

			font1.Append(fontSize1);
			font1.Append(color1);
			font1.Append(fontName1);
			font1.Append(fontFamilyNumbering1);
			font1.Append(fontScheme1);

			var font2 = new Font();
			var fontSize2 = new FontSize { Val = 11D };
			var color2 = new Color { Indexed = 8U };
			var fontName2 = new FontName { Val = "Arial" };
			var fontFamilyNumbering2 = new FontFamilyNumbering { Val = 2 };

			font2.Append(fontSize2);
			font2.Append(color2);
			font2.Append(fontName2);
			font2.Append(fontFamilyNumbering2);

			var font3 = new Font();
			var fontSize3 = new FontSize { Val = 11D };
			var color3 = new Color { Theme = 1U };
			var fontName3 = new FontName { Val = "Arial" };
			var fontFamilyNumbering3 = new FontFamilyNumbering { Val = 2 };
			var fontScheme2 = new FontScheme { Val = FontSchemeValues.Minor };

			font3.Append(fontSize3);
			font3.Append(color3);
			font3.Append(fontName3);
			font3.Append(fontFamilyNumbering3);
			font3.Append(fontScheme2);

			var font4 = new Font();
			var fontSize4 = new FontSize { Val = 11D };
			var color4 = new Color { Theme = 0U };
			var fontName4 = new FontName { Val = "Arial" };
			var fontFamilyNumbering4 = new FontFamilyNumbering { Val = 2 };
			var fontScheme3 = new FontScheme { Val = FontSchemeValues.Minor };

			font4.Append(fontSize4);
			font4.Append(color4);
			font4.Append(fontName4);
			font4.Append(fontFamilyNumbering4);
			font4.Append(fontScheme3);

			var font5 = new Font();
			var fontSize5 = new FontSize { Val = 11D };
			var color5 = new Color { Rgb = "FF9C0006" };
			var fontName5 = new FontName { Val = "Arial" };
			var fontFamilyNumbering5 = new FontFamilyNumbering { Val = 2 };
			var fontScheme4 = new FontScheme { Val = FontSchemeValues.Minor };

			font5.Append(fontSize5);
			font5.Append(color5);
			font5.Append(fontName5);
			font5.Append(fontFamilyNumbering5);
			font5.Append(fontScheme4);

			var font6 = new Font();
			var bold1 = new Bold();
			var fontSize6 = new FontSize { Val = 11D };
			var color6 = new Color { Rgb = "FFFA7D00" };
			var fontName6 = new FontName { Val = "Arial" };
			var fontFamilyNumbering6 = new FontFamilyNumbering { Val = 2 };
			var fontScheme5 = new FontScheme { Val = FontSchemeValues.Minor };

			font6.Append(bold1);
			font6.Append(fontSize6);
			font6.Append(color6);
			font6.Append(fontName6);
			font6.Append(fontFamilyNumbering6);
			font6.Append(fontScheme5);

			var font7 = new Font();
			var bold2 = new Bold();
			var fontSize7 = new FontSize { Val = 11D };
			var color7 = new Color { Theme = 0U };
			var fontName7 = new FontName { Val = "Arial" };
			var fontFamilyNumbering7 = new FontFamilyNumbering { Val = 2 };
			var fontScheme6 = new FontScheme { Val = FontSchemeValues.Minor };

			font7.Append(bold2);
			font7.Append(fontSize7);
			font7.Append(color7);
			font7.Append(fontName7);
			font7.Append(fontFamilyNumbering7);
			font7.Append(fontScheme6);

			var font8 = new Font();
			var italic1 = new Italic();
			var fontSize8 = new FontSize { Val = 11D };
			var color8 = new Color { Rgb = "FF7F7F7F" };
			var fontName8 = new FontName { Val = "Arial" };
			var fontFamilyNumbering8 = new FontFamilyNumbering { Val = 2 };
			var fontScheme7 = new FontScheme { Val = FontSchemeValues.Minor };

			font8.Append(italic1);
			font8.Append(fontSize8);
			font8.Append(color8);
			font8.Append(fontName8);
			font8.Append(fontFamilyNumbering8);
			font8.Append(fontScheme7);

			var font9 = new Font();
			var fontSize9 = new FontSize { Val = 11D };
			var color9 = new Color { Rgb = "FF006100" };
			var fontName9 = new FontName { Val = "Arial" };
			var fontFamilyNumbering9 = new FontFamilyNumbering { Val = 2 };
			var fontScheme8 = new FontScheme { Val = FontSchemeValues.Minor };

			font9.Append(fontSize9);
			font9.Append(color9);
			font9.Append(fontName9);
			font9.Append(fontFamilyNumbering9);
			font9.Append(fontScheme8);

			var font10 = new Font();
			var bold3 = new Bold();
			var fontSize10 = new FontSize { Val = 15D };
			var color10 = new Color { Theme = 3U };
			var fontName10 = new FontName { Val = "Arial" };
			var fontFamilyNumbering10 = new FontFamilyNumbering { Val = 2 };
			var fontScheme9 = new FontScheme { Val = FontSchemeValues.Minor };

			font10.Append(bold3);
			font10.Append(fontSize10);
			font10.Append(color10);
			font10.Append(fontName10);
			font10.Append(fontFamilyNumbering10);
			font10.Append(fontScheme9);

			var font11 = new Font();
			var bold4 = new Bold();
			var fontSize11 = new FontSize { Val = 13D };
			var color11 = new Color { Theme = 3U };
			var fontName11 = new FontName { Val = "Arial" };
			var fontFamilyNumbering11 = new FontFamilyNumbering { Val = 2 };
			var fontScheme10 = new FontScheme { Val = FontSchemeValues.Minor };

			font11.Append(bold4);
			font11.Append(fontSize11);
			font11.Append(color11);
			font11.Append(fontName11);
			font11.Append(fontFamilyNumbering11);
			font11.Append(fontScheme10);

			var font12 = new Font();
			var bold5 = new Bold();
			var fontSize12 = new FontSize { Val = 11D };
			var color12 = new Color { Theme = 3U };
			var fontName12 = new FontName { Val = "Arial" };
			var fontFamilyNumbering12 = new FontFamilyNumbering { Val = 2 };
			var fontScheme11 = new FontScheme { Val = FontSchemeValues.Minor };

			font12.Append(bold5);
			font12.Append(fontSize12);
			font12.Append(color12);
			font12.Append(fontName12);
			font12.Append(fontFamilyNumbering12);
			font12.Append(fontScheme11);

			var font13 = new Font();
			var fontSize13 = new FontSize { Val = 11D };
			var color13 = new Color { Rgb = "FF3F3F76" };
			var fontName13 = new FontName { Val = "Arial" };
			var fontFamilyNumbering13 = new FontFamilyNumbering { Val = 2 };
			var fontScheme12 = new FontScheme { Val = FontSchemeValues.Minor };

			font13.Append(fontSize13);
			font13.Append(color13);
			font13.Append(fontName13);
			font13.Append(fontFamilyNumbering13);
			font13.Append(fontScheme12);

			var font14 = new Font();
			var fontSize14 = new FontSize { Val = 11D };
			var color14 = new Color { Rgb = "FFFA7D00" };
			var fontName14 = new FontName { Val = "Arial" };
			var fontFamilyNumbering14 = new FontFamilyNumbering { Val = 2 };
			var fontScheme13 = new FontScheme { Val = FontSchemeValues.Minor };

			font14.Append(fontSize14);
			font14.Append(color14);
			font14.Append(fontName14);
			font14.Append(fontFamilyNumbering14);
			font14.Append(fontScheme13);

			var font15 = new Font();
			var fontSize15 = new FontSize { Val = 11D };
			var color15 = new Color { Rgb = "FF9C6500" };
			var fontName15 = new FontName { Val = "Arial" };
			var fontFamilyNumbering15 = new FontFamilyNumbering { Val = 2 };
			var fontScheme14 = new FontScheme { Val = FontSchemeValues.Minor };

			font15.Append(fontSize15);
			font15.Append(color15);
			font15.Append(fontName15);
			font15.Append(fontFamilyNumbering15);
			font15.Append(fontScheme14);

			var font16 = new Font();
			var bold6 = new Bold();
			var fontSize16 = new FontSize { Val = 11D };
			var color16 = new Color { Rgb = "FF3F3F3F" };
			var fontName16 = new FontName { Val = "Arial" };
			var fontFamilyNumbering16 = new FontFamilyNumbering { Val = 2 };
			var fontScheme15 = new FontScheme { Val = FontSchemeValues.Minor };

			font16.Append(bold6);
			font16.Append(fontSize16);
			font16.Append(color16);
			font16.Append(fontName16);
			font16.Append(fontFamilyNumbering16);
			font16.Append(fontScheme15);

			var font17 = new Font();
			var bold7 = new Bold();
			var fontSize17 = new FontSize { Val = 18D };
			var color17 = new Color { Theme = 3U };
			var fontName17 = new FontName { Val = "Cambria" };
			var fontFamilyNumbering17 = new FontFamilyNumbering { Val = 2 };
			var fontScheme16 = new FontScheme { Val = FontSchemeValues.Major };

			font17.Append(bold7);
			font17.Append(fontSize17);
			font17.Append(color17);
			font17.Append(fontName17);
			font17.Append(fontFamilyNumbering17);
			font17.Append(fontScheme16);

			var font18 = new Font();
			var bold8 = new Bold();
			var fontSize18 = new FontSize { Val = 11D };
			var color18 = new Color { Theme = 1U };
			var fontName18 = new FontName { Val = "Arial" };
			var fontFamilyNumbering18 = new FontFamilyNumbering { Val = 2 };
			var fontScheme17 = new FontScheme { Val = FontSchemeValues.Minor };

			font18.Append(bold8);
			font18.Append(fontSize18);
			font18.Append(color18);
			font18.Append(fontName18);
			font18.Append(fontFamilyNumbering18);
			font18.Append(fontScheme17);

			var font19 = new Font();
			var fontSize19 = new FontSize { Val = 11D };
			var color19 = new Color { Rgb = "FFFF0000" };
			var fontName19 = new FontName { Val = "Arial" };
			var fontFamilyNumbering19 = new FontFamilyNumbering { Val = 2 };
			var fontScheme18 = new FontScheme { Val = FontSchemeValues.Minor };

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

			var fills1 = new Fills { Count = 33U };

			var fill1 = new Fill();
			var patternFill1 = new PatternFill { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			var fill2 = new Fill();
			var patternFill2 = new PatternFill { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			var fill3 = new Fill();

			var patternFill3 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor1 = new ForegroundColor { Theme = 4U, Tint = 0.79998168889431442D };
			var backgroundColor1 = new BackgroundColor { Indexed = 65U };

			patternFill3.Append(foregroundColor1);
			patternFill3.Append(backgroundColor1);

			fill3.Append(patternFill3);

			var fill4 = new Fill();

			var patternFill4 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor2 = new ForegroundColor { Theme = 5U, Tint = 0.79998168889431442D };
			var backgroundColor2 = new BackgroundColor { Indexed = 65U };

			patternFill4.Append(foregroundColor2);
			patternFill4.Append(backgroundColor2);

			fill4.Append(patternFill4);

			var fill5 = new Fill();

			var patternFill5 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor3 = new ForegroundColor { Theme = 6U, Tint = 0.79998168889431442D };
			var backgroundColor3 = new BackgroundColor { Indexed = 65U };

			patternFill5.Append(foregroundColor3);
			patternFill5.Append(backgroundColor3);

			fill5.Append(patternFill5);

			var fill6 = new Fill();

			var patternFill6 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor4 = new ForegroundColor { Theme = 7U, Tint = 0.79998168889431442D };
			var backgroundColor4 = new BackgroundColor { Indexed = 65U };

			patternFill6.Append(foregroundColor4);
			patternFill6.Append(backgroundColor4);

			fill6.Append(patternFill6);

			var fill7 = new Fill();

			var patternFill7 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor5 = new ForegroundColor { Theme = 8U, Tint = 0.79998168889431442D };
			var backgroundColor5 = new BackgroundColor { Indexed = 65U };

			patternFill7.Append(foregroundColor5);
			patternFill7.Append(backgroundColor5);

			fill7.Append(patternFill7);

			var fill8 = new Fill();

			var patternFill8 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor6 = new ForegroundColor { Theme = 9U, Tint = 0.79998168889431442D };
			var backgroundColor6 = new BackgroundColor { Indexed = 65U };

			patternFill8.Append(foregroundColor6);
			patternFill8.Append(backgroundColor6);

			fill8.Append(patternFill8);

			var fill9 = new Fill();

			var patternFill9 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor7 = new ForegroundColor { Theme = 4U, Tint = 0.59999389629810485D };
			var backgroundColor7 = new BackgroundColor { Indexed = 65U };

			patternFill9.Append(foregroundColor7);
			patternFill9.Append(backgroundColor7);

			fill9.Append(patternFill9);

			var fill10 = new Fill();

			var patternFill10 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor8 = new ForegroundColor { Theme = 5U, Tint = 0.59999389629810485D };
			var backgroundColor8 = new BackgroundColor { Indexed = 65U };

			patternFill10.Append(foregroundColor8);
			patternFill10.Append(backgroundColor8);

			fill10.Append(patternFill10);

			var fill11 = new Fill();

			var patternFill11 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor9 = new ForegroundColor { Theme = 6U, Tint = 0.59999389629810485D };
			var backgroundColor9 = new BackgroundColor { Indexed = 65U };

			patternFill11.Append(foregroundColor9);
			patternFill11.Append(backgroundColor9);

			fill11.Append(patternFill11);

			var fill12 = new Fill();

			var patternFill12 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor10 = new ForegroundColor { Theme = 7U, Tint = 0.59999389629810485D };
			var backgroundColor10 = new BackgroundColor { Indexed = 65U };

			patternFill12.Append(foregroundColor10);
			patternFill12.Append(backgroundColor10);

			fill12.Append(patternFill12);

			var fill13 = new Fill();

			var patternFill13 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor11 = new ForegroundColor { Theme = 8U, Tint = 0.59999389629810485D };
			var backgroundColor11 = new BackgroundColor { Indexed = 65U };

			patternFill13.Append(foregroundColor11);
			patternFill13.Append(backgroundColor11);

			fill13.Append(patternFill13);

			var fill14 = new Fill();

			var patternFill14 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor12 = new ForegroundColor { Theme = 9U, Tint = 0.59999389629810485D };
			var backgroundColor12 = new BackgroundColor { Indexed = 65U };

			patternFill14.Append(foregroundColor12);
			patternFill14.Append(backgroundColor12);

			fill14.Append(patternFill14);

			var fill15 = new Fill();

			var patternFill15 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor13 = new ForegroundColor { Theme = 4U, Tint = 0.39997558519241921D };
			var backgroundColor13 = new BackgroundColor { Indexed = 65U };

			patternFill15.Append(foregroundColor13);
			patternFill15.Append(backgroundColor13);

			fill15.Append(patternFill15);

			var fill16 = new Fill();

			var patternFill16 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor14 = new ForegroundColor { Theme = 5U, Tint = 0.39997558519241921D };
			var backgroundColor14 = new BackgroundColor { Indexed = 65U };

			patternFill16.Append(foregroundColor14);
			patternFill16.Append(backgroundColor14);

			fill16.Append(patternFill16);

			var fill17 = new Fill();

			var patternFill17 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor15 = new ForegroundColor { Theme = 6U, Tint = 0.39997558519241921D };
			var backgroundColor15 = new BackgroundColor { Indexed = 65U };

			patternFill17.Append(foregroundColor15);
			patternFill17.Append(backgroundColor15);

			fill17.Append(patternFill17);

			var fill18 = new Fill();

			var patternFill18 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor16 = new ForegroundColor { Theme = 7U, Tint = 0.39997558519241921D };
			var backgroundColor16 = new BackgroundColor { Indexed = 65U };

			patternFill18.Append(foregroundColor16);
			patternFill18.Append(backgroundColor16);

			fill18.Append(patternFill18);

			var fill19 = new Fill();

			var patternFill19 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor17 = new ForegroundColor { Theme = 8U, Tint = 0.39997558519241921D };
			var backgroundColor17 = new BackgroundColor { Indexed = 65U };

			patternFill19.Append(foregroundColor17);
			patternFill19.Append(backgroundColor17);

			fill19.Append(patternFill19);

			var fill20 = new Fill();

			var patternFill20 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor18 = new ForegroundColor { Theme = 9U, Tint = 0.39997558519241921D };
			var backgroundColor18 = new BackgroundColor { Indexed = 65U };

			patternFill20.Append(foregroundColor18);
			patternFill20.Append(backgroundColor18);

			fill20.Append(patternFill20);

			var fill21 = new Fill();

			var patternFill21 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor19 = new ForegroundColor { Theme = 4U };

			patternFill21.Append(foregroundColor19);

			fill21.Append(patternFill21);

			var fill22 = new Fill();

			var patternFill22 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor20 = new ForegroundColor { Theme = 5U };

			patternFill22.Append(foregroundColor20);

			fill22.Append(patternFill22);

			var fill23 = new Fill();

			var patternFill23 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor21 = new ForegroundColor { Theme = 6U };

			patternFill23.Append(foregroundColor21);

			fill23.Append(patternFill23);

			var fill24 = new Fill();

			var patternFill24 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor22 = new ForegroundColor { Theme = 7U };

			patternFill24.Append(foregroundColor22);

			fill24.Append(patternFill24);

			var fill25 = new Fill();

			var patternFill25 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor23 = new ForegroundColor { Theme = 8U };

			patternFill25.Append(foregroundColor23);

			fill25.Append(patternFill25);

			var fill26 = new Fill();

			var patternFill26 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor24 = new ForegroundColor { Theme = 9U };

			patternFill26.Append(foregroundColor24);

			fill26.Append(patternFill26);

			var fill27 = new Fill();

			var patternFill27 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor25 = new ForegroundColor { Rgb = "FFFFC7CE" };

			patternFill27.Append(foregroundColor25);

			fill27.Append(patternFill27);

			var fill28 = new Fill();

			var patternFill28 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor26 = new ForegroundColor { Rgb = "FFF2F2F2" };

			patternFill28.Append(foregroundColor26);

			fill28.Append(patternFill28);

			var fill29 = new Fill();

			var patternFill29 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor27 = new ForegroundColor { Rgb = "FFA5A5A5" };

			patternFill29.Append(foregroundColor27);

			fill29.Append(patternFill29);

			var fill30 = new Fill();

			var patternFill30 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor28 = new ForegroundColor { Rgb = "FFC6EFCE" };

			patternFill30.Append(foregroundColor28);

			fill30.Append(patternFill30);

			var fill31 = new Fill();

			var patternFill31 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor29 = new ForegroundColor { Rgb = "FFFFCC99" };

			patternFill31.Append(foregroundColor29);

			fill31.Append(patternFill31);

			var fill32 = new Fill();

			var patternFill32 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor30 = new ForegroundColor { Rgb = "FFFFEB9C" };

			patternFill32.Append(foregroundColor30);

			fill32.Append(patternFill32);

			var fill33 = new Fill();

			var patternFill33 = new PatternFill { PatternType = PatternValues.Solid };
			var foregroundColor31 = new ForegroundColor { Rgb = "FFFFFFCC" };

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

			var borders1 = new Borders { Count = 10U };

			var border1 = new Border();
			var leftBorder1 = new LeftBorder();
			var rightBorder1 = new RightBorder();
			var topBorder1 = new TopBorder();
			var bottomBorder1 = new BottomBorder();
			var diagonalBorder1 = new DiagonalBorder();

			border1.Append(leftBorder1);
			border1.Append(rightBorder1);
			border1.Append(topBorder1);
			border1.Append(bottomBorder1);
			border1.Append(diagonalBorder1);

			var border2 = new Border();

			var leftBorder2 = new LeftBorder { Style = BorderStyleValues.Thin };
			var color20 = new Color { Rgb = "FF7F7F7F" };

			leftBorder2.Append(color20);

			var rightBorder2 = new RightBorder { Style = BorderStyleValues.Thin };
			var color21 = new Color { Rgb = "FF7F7F7F" };

			rightBorder2.Append(color21);

			var topBorder2 = new TopBorder { Style = BorderStyleValues.Thin };
			var color22 = new Color { Rgb = "FF7F7F7F" };

			topBorder2.Append(color22);

			var bottomBorder2 = new BottomBorder { Style = BorderStyleValues.Thin };
			var color23 = new Color { Rgb = "FF7F7F7F" };

			bottomBorder2.Append(color23);
			var diagonalBorder2 = new DiagonalBorder();

			border2.Append(leftBorder2);
			border2.Append(rightBorder2);
			border2.Append(topBorder2);
			border2.Append(bottomBorder2);
			border2.Append(diagonalBorder2);

			var border3 = new Border();

			var leftBorder3 = new LeftBorder { Style = BorderStyleValues.Double };
			var color24 = new Color { Rgb = "FF3F3F3F" };

			leftBorder3.Append(color24);

			var rightBorder3 = new RightBorder { Style = BorderStyleValues.Double };
			var color25 = new Color { Rgb = "FF3F3F3F" };

			rightBorder3.Append(color25);

			var topBorder3 = new TopBorder { Style = BorderStyleValues.Double };
			var color26 = new Color { Rgb = "FF3F3F3F" };

			topBorder3.Append(color26);

			var bottomBorder3 = new BottomBorder { Style = BorderStyleValues.Double };
			var color27 = new Color { Rgb = "FF3F3F3F" };

			bottomBorder3.Append(color27);
			var diagonalBorder3 = new DiagonalBorder();

			border3.Append(leftBorder3);
			border3.Append(rightBorder3);
			border3.Append(topBorder3);
			border3.Append(bottomBorder3);
			border3.Append(diagonalBorder3);

			var border4 = new Border();
			var leftBorder4 = new LeftBorder();
			var rightBorder4 = new RightBorder();
			var topBorder4 = new TopBorder();

			var bottomBorder4 = new BottomBorder { Style = BorderStyleValues.Thick };
			var color28 = new Color { Theme = 4U };

			bottomBorder4.Append(color28);
			var diagonalBorder4 = new DiagonalBorder();

			border4.Append(leftBorder4);
			border4.Append(rightBorder4);
			border4.Append(topBorder4);
			border4.Append(bottomBorder4);
			border4.Append(diagonalBorder4);

			var border5 = new Border();
			var leftBorder5 = new LeftBorder();
			var rightBorder5 = new RightBorder();
			var topBorder5 = new TopBorder();

			var bottomBorder5 = new BottomBorder { Style = BorderStyleValues.Thick };
			var color29 = new Color { Theme = 4U, Tint = 0.499984740745262D };

			bottomBorder5.Append(color29);
			var diagonalBorder5 = new DiagonalBorder();

			border5.Append(leftBorder5);
			border5.Append(rightBorder5);
			border5.Append(topBorder5);
			border5.Append(bottomBorder5);
			border5.Append(diagonalBorder5);

			var border6 = new Border();
			var leftBorder6 = new LeftBorder();
			var rightBorder6 = new RightBorder();
			var topBorder6 = new TopBorder();

			var bottomBorder6 = new BottomBorder { Style = BorderStyleValues.Medium };
			var color30 = new Color { Theme = 4U, Tint = 0.39997558519241921D };

			bottomBorder6.Append(color30);
			var diagonalBorder6 = new DiagonalBorder();

			border6.Append(leftBorder6);
			border6.Append(rightBorder6);
			border6.Append(topBorder6);
			border6.Append(bottomBorder6);
			border6.Append(diagonalBorder6);

			var border7 = new Border();
			var leftBorder7 = new LeftBorder();
			var rightBorder7 = new RightBorder();
			var topBorder7 = new TopBorder();

			var bottomBorder7 = new BottomBorder { Style = BorderStyleValues.Double };
			var color31 = new Color { Rgb = "FFFF8001" };

			bottomBorder7.Append(color31);
			var diagonalBorder7 = new DiagonalBorder();

			border7.Append(leftBorder7);
			border7.Append(rightBorder7);
			border7.Append(topBorder7);
			border7.Append(bottomBorder7);
			border7.Append(diagonalBorder7);

			var border8 = new Border();

			var leftBorder8 = new LeftBorder { Style = BorderStyleValues.Thin };
			var color32 = new Color { Rgb = "FFB2B2B2" };

			leftBorder8.Append(color32);

			var rightBorder8 = new RightBorder { Style = BorderStyleValues.Thin };
			var color33 = new Color { Rgb = "FFB2B2B2" };

			rightBorder8.Append(color33);

			var topBorder8 = new TopBorder { Style = BorderStyleValues.Thin };
			var color34 = new Color { Rgb = "FFB2B2B2" };

			topBorder8.Append(color34);

			var bottomBorder8 = new BottomBorder { Style = BorderStyleValues.Thin };
			var color35 = new Color { Rgb = "FFB2B2B2" };

			bottomBorder8.Append(color35);
			var diagonalBorder8 = new DiagonalBorder();

			border8.Append(leftBorder8);
			border8.Append(rightBorder8);
			border8.Append(topBorder8);
			border8.Append(bottomBorder8);
			border8.Append(diagonalBorder8);

			var border9 = new Border();

			var leftBorder9 = new LeftBorder { Style = BorderStyleValues.Thin };
			var color36 = new Color { Rgb = "FF3F3F3F" };

			leftBorder9.Append(color36);

			var rightBorder9 = new RightBorder { Style = BorderStyleValues.Thin };
			var color37 = new Color { Rgb = "FF3F3F3F" };

			rightBorder9.Append(color37);

			var topBorder9 = new TopBorder { Style = BorderStyleValues.Thin };
			var color38 = new Color { Rgb = "FF3F3F3F" };

			topBorder9.Append(color38);

			var bottomBorder9 = new BottomBorder { Style = BorderStyleValues.Thin };
			var color39 = new Color { Rgb = "FF3F3F3F" };

			bottomBorder9.Append(color39);
			var diagonalBorder9 = new DiagonalBorder();

			border9.Append(leftBorder9);
			border9.Append(rightBorder9);
			border9.Append(topBorder9);
			border9.Append(bottomBorder9);
			border9.Append(diagonalBorder9);

			var border10 = new Border();
			var leftBorder10 = new LeftBorder();
			var rightBorder10 = new RightBorder();

			var topBorder10 = new TopBorder { Style = BorderStyleValues.Thin };
			var color40 = new Color { Theme = 4U };

			topBorder10.Append(color40);

			var bottomBorder10 = new BottomBorder { Style = BorderStyleValues.Double };
			var color41 = new Color { Theme = 4U };

			bottomBorder10.Append(color41);
			var diagonalBorder10 = new DiagonalBorder();

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

			var cellStyleFormats1 = new CellStyleFormats { Count = 42U };
			var cellFormat1 = new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U };
			var cellFormat2 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 2U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat3 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 3U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat4 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 4U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat5 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 5U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat6 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 6U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat7 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 7U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat8 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 8U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat9 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 9U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat10 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 10U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat11 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 11U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat12 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 12U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat13 = new CellFormat { NumberFormatId = 0U, FontId = 2U, FillId = 13U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat14 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 14U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat15 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 15U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat16 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 16U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat17 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 17U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat18 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 18U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat19 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 19U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat20 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 20U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat21 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 21U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat22 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 22U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat23 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 23U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat24 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 24U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat25 = new CellFormat { NumberFormatId = 0U, FontId = 3U, FillId = 25U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat26 = new CellFormat { NumberFormatId = 0U, FontId = 4U, FillId = 26U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat27 = new CellFormat { NumberFormatId = 0U, FontId = 5U, FillId = 27U, BorderId = 1U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat28 = new CellFormat { NumberFormatId = 0U, FontId = 6U, FillId = 28U, BorderId = 2U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat29 = new CellFormat { NumberFormatId = 0U, FontId = 7U, FillId = 0U, BorderId = 0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat30 = new CellFormat { NumberFormatId = 0U, FontId = 8U, FillId = 29U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat31 = new CellFormat { NumberFormatId = 0U, FontId = 9U, FillId = 0U, BorderId = 3U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat32 = new CellFormat { NumberFormatId = 0U, FontId = 10U, FillId = 0U, BorderId = 4U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat33 = new CellFormat { NumberFormatId = 0U, FontId = 11U, FillId = 0U, BorderId = 5U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat34 = new CellFormat { NumberFormatId = 0U, FontId = 11U, FillId = 0U, BorderId = 0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat35 = new CellFormat { NumberFormatId = 0U, FontId = 12U, FillId = 30U, BorderId = 1U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat36 = new CellFormat { NumberFormatId = 0U, FontId = 13U, FillId = 0U, BorderId = 6U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat37 = new CellFormat { NumberFormatId = 0U, FontId = 14U, FillId = 31U, BorderId = 0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat38 = new CellFormat { NumberFormatId = 0U, FontId = 1U, FillId = 32U, BorderId = 7U, ApplyNumberFormat = false, ApplyFont = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat39 = new CellFormat { NumberFormatId = 0U, FontId = 15U, FillId = 27U, BorderId = 8U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat40 = new CellFormat { NumberFormatId = 0U, FontId = 16U, FillId = 0U, BorderId = 0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat41 = new CellFormat { NumberFormatId = 0U, FontId = 17U, FillId = 0U, BorderId = 9U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
			var cellFormat42 = new CellFormat { NumberFormatId = 0U, FontId = 18U, FillId = 0U, BorderId = 0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };

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

			var cellFormats1 = new CellFormats { Count = 2U };
			var cellFormat43 = new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U, FormatId = 0U };
			var cellFormat44 = new CellFormat { NumberFormatId = 14U, FontId = 0U, FillId = 0U, BorderId = 0U, FormatId = 0U, ApplyNumberFormat = true };

			cellFormats1.Append(cellFormat43);
			cellFormats1.Append(cellFormat44);

			var cellStyles1 = new CellStyles { Count = 42U };
			var cellStyle1 = new CellStyle { Name = "20% - Accent1", FormatId = 1U, BuiltinId = 30U, CustomBuiltin = true };
			var cellStyle2 = new CellStyle { Name = "20% - Accent2", FormatId = 2U, BuiltinId = 34U, CustomBuiltin = true };
			var cellStyle3 = new CellStyle { Name = "20% - Accent3", FormatId = 3U, BuiltinId = 38U, CustomBuiltin = true };
			var cellStyle4 = new CellStyle { Name = "20% - Accent4", FormatId = 4U, BuiltinId = 42U, CustomBuiltin = true };
			var cellStyle5 = new CellStyle { Name = "20% - Accent5", FormatId = 5U, BuiltinId = 46U, CustomBuiltin = true };
			var cellStyle6 = new CellStyle { Name = "20% - Accent6", FormatId = 6U, BuiltinId = 50U, CustomBuiltin = true };
			var cellStyle7 = new CellStyle { Name = "40% - Accent1", FormatId = 7U, BuiltinId = 31U, CustomBuiltin = true };
			var cellStyle8 = new CellStyle { Name = "40% - Accent2", FormatId = 8U, BuiltinId = 35U, CustomBuiltin = true };
			var cellStyle9 = new CellStyle { Name = "40% - Accent3", FormatId = 9U, BuiltinId = 39U, CustomBuiltin = true };
			var cellStyle10 = new CellStyle { Name = "40% - Accent4", FormatId = 10U, BuiltinId = 43U, CustomBuiltin = true };
			var cellStyle11 = new CellStyle { Name = "40% - Accent5", FormatId = 11U, BuiltinId = 47U, CustomBuiltin = true };
			var cellStyle12 = new CellStyle { Name = "40% - Accent6", FormatId = 12U, BuiltinId = 51U, CustomBuiltin = true };
			var cellStyle13 = new CellStyle { Name = "60% - Accent1", FormatId = 13U, BuiltinId = 32U, CustomBuiltin = true };
			var cellStyle14 = new CellStyle { Name = "60% - Accent2", FormatId = 14U, BuiltinId = 36U, CustomBuiltin = true };
			var cellStyle15 = new CellStyle { Name = "60% - Accent3", FormatId = 15U, BuiltinId = 40U, CustomBuiltin = true };
			var cellStyle16 = new CellStyle { Name = "60% - Accent4", FormatId = 16U, BuiltinId = 44U, CustomBuiltin = true };
			var cellStyle17 = new CellStyle { Name = "60% - Accent5", FormatId = 17U, BuiltinId = 48U, CustomBuiltin = true };
			var cellStyle18 = new CellStyle { Name = "60% - Accent6", FormatId = 18U, BuiltinId = 52U, CustomBuiltin = true };
			var cellStyle19 = new CellStyle { Name = "Accent1", FormatId = 19U, BuiltinId = 29U, CustomBuiltin = true };
			var cellStyle20 = new CellStyle { Name = "Accent2", FormatId = 20U, BuiltinId = 33U, CustomBuiltin = true };
			var cellStyle21 = new CellStyle { Name = "Accent3", FormatId = 21U, BuiltinId = 37U, CustomBuiltin = true };
			var cellStyle22 = new CellStyle { Name = "Accent4", FormatId = 22U, BuiltinId = 41U, CustomBuiltin = true };
			var cellStyle23 = new CellStyle { Name = "Accent5", FormatId = 23U, BuiltinId = 45U, CustomBuiltin = true };
			var cellStyle24 = new CellStyle { Name = "Accent6", FormatId = 24U, BuiltinId = 49U, CustomBuiltin = true };
			var cellStyle25 = new CellStyle { Name = "Bad", FormatId = 25U, BuiltinId = 27U, CustomBuiltin = true };
			var cellStyle26 = new CellStyle { Name = "Calculation", FormatId = 26U, BuiltinId = 22U, CustomBuiltin = true };
			var cellStyle27 = new CellStyle { Name = "Check Cell", FormatId = 27U, BuiltinId = 23U, CustomBuiltin = true };
			var cellStyle28 = new CellStyle { Name = "Explanatory Text", FormatId = 28U, BuiltinId = 53U, CustomBuiltin = true };
			var cellStyle29 = new CellStyle { Name = "Good", FormatId = 29U, BuiltinId = 26U, CustomBuiltin = true };
			var cellStyle30 = new CellStyle { Name = "Heading 1", FormatId = 30U, BuiltinId = 16U, CustomBuiltin = true };
			var cellStyle31 = new CellStyle { Name = "Heading 2", FormatId = 31U, BuiltinId = 17U, CustomBuiltin = true };
			var cellStyle32 = new CellStyle { Name = "Heading 3", FormatId = 32U, BuiltinId = 18U, CustomBuiltin = true };
			var cellStyle33 = new CellStyle { Name = "Heading 4", FormatId = 33U, BuiltinId = 19U, CustomBuiltin = true };
			var cellStyle34 = new CellStyle { Name = "Input", FormatId = 34U, BuiltinId = 20U, CustomBuiltin = true };
			var cellStyle35 = new CellStyle { Name = "Linked Cell", FormatId = 35U, BuiltinId = 24U, CustomBuiltin = true };
			var cellStyle36 = new CellStyle { Name = "Neutral", FormatId = 36U, BuiltinId = 28U, CustomBuiltin = true };
			var cellStyle37 = new CellStyle { Name = "Normal", FormatId = 0U, BuiltinId = 0U };
			var cellStyle38 = new CellStyle { Name = "Note", FormatId = 37U, BuiltinId = 10U, CustomBuiltin = true };
			var cellStyle39 = new CellStyle { Name = "Output", FormatId = 38U, BuiltinId = 21U, CustomBuiltin = true };
			var cellStyle40 = new CellStyle { Name = "Title", FormatId = 39U, BuiltinId = 15U, CustomBuiltin = true };
			var cellStyle41 = new CellStyle { Name = "Total", FormatId = 40U, BuiltinId = 25U, CustomBuiltin = true };
			var cellStyle42 = new CellStyle { Name = "Warning Text", FormatId = 41U, BuiltinId = 11U, CustomBuiltin = true };

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
			var differentialFormats1 = new DifferentialFormats { Count = 0U };
			var tableStyles1 = new TableStyles { Count = 0U, DefaultTableStyle = "TableStyleMedium9", DefaultPivotStyle = "PivotStyleLight16" };

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
