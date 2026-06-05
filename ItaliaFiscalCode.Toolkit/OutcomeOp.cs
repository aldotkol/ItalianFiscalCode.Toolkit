namespace ItaliaFiscalCode.Toolkit
{
	public class OutcomeOp
	{
		private bool _outcome;
		private string _response;

		public bool Outcome { get => _outcome; set => _outcome = value; }
		public string Response { get => _response; set => _response = value; }
		public OutcomeOp() {
			_outcome = false;
			_response = "";
		}
		public static OutcomeOp Success()=> new OutcomeOp() { Outcome = true };
		public static OutcomeOp Success(string m)=> new OutcomeOp() { Outcome = true,Response=m };
		public static OutcomeOp Fail() => new OutcomeOp() { Outcome = false };
		public static OutcomeOp Fail(string m) => new OutcomeOp() { Outcome = false, Response = m };
	}
}
