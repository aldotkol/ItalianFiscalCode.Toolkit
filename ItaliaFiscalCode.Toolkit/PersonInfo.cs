namespace ItaliaFiscalCode.Toolkit
{
	public class PersonInfo
	{
		private string _name;
		private string _surname;
		private string _birthDate;
		private string _gender;
		private string _birthPlaceCode;

		public string Name { get => _name; set => _name = value; }
		public string Surname { get => _surname; set => _surname = value; }
		public string BirthDate { get => _birthDate; set => _birthDate = value; }
		public string Gender { get => _gender; set => _gender = value; }
		public string BirthPlaceCode { get => _birthPlaceCode; set => _birthPlaceCode = value; }
		/// <summary>
		/// Costruttore iniziale, con dati da valorizzare
		/// </summary>
		public PersonInfo() {
			_name = "";
			_surname = "";
			_birthDate = "";
			_gender = "";
			_birthPlaceCode = "";
		}
		/// <summary>
		/// Costruttore popolato con i dati impostati
		/// </summary>
		/// <param name="name"></param>
		/// <param name="surname"></param>
		/// <param name="birthDate"></param>
		/// <param name="gender"></param>
		/// <param name="birthPlaceCode"></param>
		public PersonInfo(string name, string surname, string birthDate, string gender, string birthPlaceCode)
		{
			_name = name;
			_surname = surname;
			_birthDate = birthDate;
			_gender = gender;
			_birthPlaceCode = birthPlaceCode;
		}
	}
}
