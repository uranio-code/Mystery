using Mystery.Register;
[GlobalAvalibleObject()]
public interface IClientProgress
{
	string message { get; set; }

	double progress { get; set; }
	void show();

	void close();
}
