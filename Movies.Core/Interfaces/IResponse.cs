namespace Movies.Core.Interfaces
{
	public interface IResponse
	{
		public int StatusCode { get; }

		public string? Message { get; set; }

		public bool Status { get; }
	}
}
