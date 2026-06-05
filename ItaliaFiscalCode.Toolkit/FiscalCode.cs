using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItaliaFiscalCode.Toolkit
{
	public class FiscalCode
	{
		private const string pattern = "^[A-Za-z]{6}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{3}[A-Za-z]{1}$";
		private const string homocodes = "LMNPQRSTUV";
		private const string allLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string vocals = "AEIOU";
		public FiscalCode() { }

		#region validation
		/// <summary>
		/// Verify last char of Fiscal Code
		/// </summary>
		/// <param name="fiscalCode">Fiscal Code</param>
		/// <returns>True if ok</returns>
		public static bool CheckDigit(string fiscalCode)
		{
			try
			{
				Regex rx = new Regex(pattern);
				if (rx.IsMatch(fiscalCode))
				{
					string firstLetters = fiscalCode.Substring(0, 15);
					string lastChar = fiscalCode.Substring(15, 1);
					string realLastChar = CreateControlChar(firstLetters);
					if (realLastChar != lastChar)
						return false;
					else
						return true;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
		}
		private static string CreateControlChar(string firstPartCode)
		{
			Hashtable evenList = EvenHashes;
			Hashtable oddList = OddHashes;
			const string controlList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int sum = 0;
			for (int i = 0; i < 15; i++)
			{
				int currVal = 0;
				string currCar = firstPartCode.Substring(i, 1);
				if ((i % 2) == 0)
					currVal = Convert.ToInt32(oddList[currCar]);
				else
					currVal = Convert.ToInt32(evenList[currCar]);
				sum += currVal;

			}

			string character = controlList.Substring(sum % 26, 1);

			return character;
		}
		
		#endregion

		#region match
		/// <summary>
		/// Verify if Fiscal Code match with Person Data
		/// </summary>
		/// <param name="_info">Person Data</param>
		/// <param name="fiscalCode">Fiscal Code</param>
		/// <returns>True if ok</returns>
		public static bool MatchPerson(PersonInfo _info, string fiscalCode)
		{

			try
			{

				if (fiscalCode.Length != 16)
					return false;

				#region fiscal code chars
				string surnameChars = fiscalCode.Substring(0, 3);
				string nameChars = fiscalCode.Substring(3, 3);
				string yearChars = fiscalCode.Substring(6, 2);
				string monthChars = fiscalCode.Substring(8, 1);
				string dayChars = fiscalCode.Substring(9, 2);
				string cityCodeChars = fiscalCode.Substring(11, 4);
				string ctrlChar = fiscalCode.Substring(15, 1);
				string fiscalCodeVerify = "";
				#endregion

				Regex rx = new Regex(pattern);
				if (!rx.IsMatch(fiscalCode))
					return false;

				#region confronto
				OutcomeOp check = HomocodeVerify(fiscalCode);
				if (check.Outcome)
				{
					fiscalCodeVerify = check.Response;
					surnameChars = fiscalCodeVerify.Substring(0, 3);
					nameChars = fiscalCodeVerify.Substring(3, 3);
					yearChars = fiscalCodeVerify.Substring(6, 2);
					monthChars = fiscalCodeVerify.Substring(8, 1);
					dayChars = fiscalCodeVerify.Substring(9, 2);
					cityCodeChars = fiscalCodeVerify.Substring(11, 4);
					ctrlChar = fiscalCodeVerify.Substring(15, 1);
				}
				else
					fiscalCodeVerify = fiscalCode;

				if (!CheckDigit(fiscalCodeVerify))
					return false;

				if (!CheckDate(_info.BirthDate))
					return false;

				DateTime birthDate = Convert.ToDateTime(_info.BirthDate);

				string surnameRes = GetSurnameChars(_info.Surname);
				string nameRes = GetNameChars(_info.Name);
				string yearRes = birthDate.Year.ToString().Substring(2, 2);
				string monthRes = GetMonthLetter(birthDate.Month);
				string dayRes = birthDate.Day.ToString("00");
				if (_info.Gender == "F")
					dayRes = (int.Parse(dayRes) + 40).ToString();

				string cityCodeRes = _info.BirthPlaceCode;
				string cfTmp = surnameRes + nameRes + yearRes + monthRes + dayRes + cityCodeRes;
				string ctrlRes = CreateControlChar(cfTmp);

				bool res = surnameRes == surnameChars;
				res = res && (nameRes == nameChars);
				res = res && (yearRes == yearChars);
				res = res && (monthRes == monthChars);
				res = res && (dayRes == dayChars);
				res = res && (cityCodeRes == cityCodeChars);
				res = res && (ctrlRes == ctrlChar);

				return res;

				#endregion
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region build
		/// <summary>
		/// Build Fiscal Code with Person Data
		/// </summary>
		/// <param name="_info">Person data</param>
		/// <returns>Outcome of operation</returns>
		public static OutcomeOp BuildFiscalCode(PersonInfo _info)
		{
			OutcomeOp res = new OutcomeOp();
			try
			{
				#region name and surname
				string surnameChars = GetSurnameChars(RemoveApos(_info.Surname));
				string nameChars = GetNameChars(RemoveApos(_info.Name));
				#endregion

				if (!CheckDate(_info.BirthDate))
					return OutcomeOp.Fail("Birth date not valid");

				DateTime birthDate = Convert.ToDateTime(_info.BirthDate);

				#region birth date
				string yearChars = birthDate.Year.ToString().Substring(2, 2);
				string monthChars = GetMonthLetter(birthDate.Month);
				string dayChars = birthDate.Day.ToString("00");
				if (_info.Gender == "F")
					dayChars = (birthDate.Day + 40).ToString();
				#endregion

				#region city code
				string cityCodeChars = _info.BirthPlaceCode;
				#endregion

				#region control char
				string firstPart = surnameChars + nameChars + yearChars + monthChars + dayChars + cityCodeChars;
				string ctrlChar = CreateControlChar(firstPart);

				string fiscalCode = firstPart + ctrlChar;

				#endregion

				return OutcomeOp.Success(fiscalCode);
			}
			catch (Exception ex)
			{
				return OutcomeOp.Fail(ex.Message);
			}
		}
		#endregion

		#region commons
		private static Hashtable EvenHashes
		{
			get
			{
				Hashtable hash = new Hashtable();
				hash.Clear();
				hash.Add("0", 0);
				hash.Add("1", 1);
				hash.Add("2", 2);
				hash.Add("3", 3);
				hash.Add("4", 4);
				hash.Add("5", 5);
				hash.Add("6", 6);
				hash.Add("7", 7);
				hash.Add("8", 8);
				hash.Add("9", 9);
				hash.Add("A", 0);
				hash.Add("B", 1);
				hash.Add("C", 2);
				hash.Add("D", 3);
				hash.Add("E", 4);
				hash.Add("F", 5);
				hash.Add("G", 6);
				hash.Add("H", 7);
				hash.Add("I", 8);
				hash.Add("J", 9);
				hash.Add("K", 10);
				hash.Add("L", 11);
				hash.Add("M", 12);
				hash.Add("N", 13);
				hash.Add("O", 14);
				hash.Add("P", 15);
				hash.Add("Q", 16);
				hash.Add("R", 17);
				hash.Add("S", 18);
				hash.Add("T", 19);
				hash.Add("U", 20);
				hash.Add("V", 21);
				hash.Add("W", 22);
				hash.Add("X", 23);
				hash.Add("Y", 24);
				hash.Add("Z", 25);
				return hash;
			}
		}
		private static Hashtable OddHashes
		{
			get
			{
				Hashtable hash = new Hashtable();
				hash.Clear();
				hash.Add("0", 1);
				hash.Add("1", 0);
				hash.Add("2", 5);
				hash.Add("3", 7);
				hash.Add("4", 9);
				hash.Add("5", 13);
				hash.Add("6", 15);
				hash.Add("7", 17);
				hash.Add("8", 19);
				hash.Add("9", 21);
				hash.Add("A", 1);
				hash.Add("B", 0);
				hash.Add("C", 5);
				hash.Add("D", 7);
				hash.Add("E", 9);
				hash.Add("F", 13);
				hash.Add("G", 15);
				hash.Add("H", 17);
				hash.Add("I", 19);
				hash.Add("J", 21);
				hash.Add("K", 2);
				hash.Add("L", 4);
				hash.Add("M", 18);
				hash.Add("N", 20);
				hash.Add("O", 11);
				hash.Add("P", 3);
				hash.Add("Q", 6);
				hash.Add("R", 8);
				hash.Add("S", 12);
				hash.Add("T", 14);
				hash.Add("U", 16);
				hash.Add("V", 10);
				hash.Add("W", 22);
				hash.Add("X", 25);
				hash.Add("Y", 24);
				hash.Add("Z", 23);
				return hash;
			}
		}
		private static string RemoveApos(string input)
		{
			input = input.ToLower();
			input = input.Replace("à", "A");
			input = input.Replace("ò", "O");
			input = input.Replace("ì", "I");
			input = input.Replace("ù", "U");
			input = input.Replace("è", "E");
			input = input.Replace("é", "E");
			input = input.ToUpper();
			return input;
		}
		private static OutcomeOp HomocodeVerify(string cf)
		{
			OutcomeOp res = new OutcomeOp();
			try
			{
				cf = cf.ToUpper();
				char[] cfChars = cf.ToCharArray();
				int x = 0;
				for (int i = 6; i < 15; i++)
				{
					if (i != 8 && i != 11)
					{
						x = homocodes.IndexOf(cfChars[i]);

						if (x != -1)
						{
							cfChars[i] = x.ToString().ToCharArray()[0];
							res.Outcome = true;
						}
					}
				}
				string fiscalCodeNoHomo = "";

				for (int i = 0; i < 15; i++)
					fiscalCodeNoHomo += cfChars[i].ToString().Trim().ToUpper();

				string ctrlChar = CreateControlChar(fiscalCodeNoHomo);
				fiscalCodeNoHomo += ctrlChar;
				res.Response = fiscalCodeNoHomo;

				return res;
			}
			catch (Exception ex)
			{
				return OutcomeOp.Fail(ex.Message);
			}
		}
		private static string GetSurnameChars(string surname)
		{
			surname = surname.Trim();
			surname = surname.ToUpper();

			string noVocalsChecks = "";
			string vocalChecks = "";

			for (int z = 0; z < surname.Length; z++)
			{
				if (allLetters.IndexOf(surname.Substring(z, 1)) > -1)
				{
					if (vocals.IndexOf(surname.Substring(z, 1)) == -1)
						noVocalsChecks += surname.Substring(z, 1);
					else
						vocalChecks += surname.Substring(z, 1);
				}
			}

			string letters;
			if (noVocalsChecks.Length >= 3)
			{
				letters = noVocalsChecks.Substring(0, 3);
			}
			else
			{
				letters = noVocalsChecks;
				letters += vocalChecks;
				if (letters.Length >= 3)
				{
					letters = letters.Substring(0, 3);
				}
				else
				{
					string ics = "";
					int countIcs = 3 - letters.Length;
					for (int cnt = 0; cnt < countIcs; cnt++)
						ics += "X";

					letters += ics;
					//per sicurezza
					letters = letters.Substring(0, 3);
				}
			}
			return letters;
		}
		private static string GetNameChars(string name)
		{
			name = name.Trim();
			name = name.ToUpper();

			string noVocalsChecks = "";
			string vocalsChecks = "";
			string noVocalsDis = "";

			for (int z = 0; z < name.Length; z++)
			{
				if (allLetters.IndexOf(name.Substring(z, 1)) > -1)
				{
					if (vocals.IndexOf(name.Substring(z, 1)) == -1)
						noVocalsChecks += name.Substring(z, 1);
					else
						vocalsChecks += name.Substring(z, 1);
				}
			}
			string letters;

			if (noVocalsChecks.Length >= 3)
			{
				if (noVocalsChecks.Length > 3)
				{
					for (int c = 0; c < noVocalsChecks.Length; c++)
					{
						if (c != 1)
							noVocalsDis += noVocalsChecks[c].ToString();

					}
					letters = noVocalsDis.Substring(0, 3);
				}
				else
				{
					letters = noVocalsChecks;
				}
			}
			else
			{
				letters = noVocalsChecks;
				letters += vocalsChecks;
				if (letters.Length >= 3)
					letters = letters.Substring(0, 3);
				else
				{
					string ics = "";
					int icsCount = 3 - letters.Length;
					for (int cnt = 0; cnt < icsCount; cnt++)
					{
						ics += "X";
					}
					letters += ics;
					letters = letters.Substring(0, 3);
				}
			}
			return letters;
		}
		private static bool CheckDate(string date)
		{
			try
			{
				DateTime dateCheck = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

				dateCheck = Convert.ToDateTime(date);
				return true;
			}
			catch
			{
				return false;
			}
		}
		private static string GetMonthLetter(int mese)
		{
			string letter = "";

			switch (mese)
			{
				case 1:
					letter = "A";
					break;
				case 2:
					letter = "B";
					break;
				case 3:
					letter = "C";
					break;
				case 4:
					letter = "D";
					break;
				case 5:
					letter = "E";
					break;
				case 6:
					letter = "H";
					break;
				case 7:
					letter = "L";
					break;
				case 8:
					letter = "M";
					break;
				case 9:
					letter = "P";
					break;
				case 10:
					letter = "R";
					break;
				case 11:
					letter = "S";
					break;
				case 12:
					letter = "T";
					break;
			}
			return letter;
		}
		#endregion
	}
}
