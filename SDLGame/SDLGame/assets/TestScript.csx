
class InScriptGameObj : BaseGameObj
{
	public override string SayHello()
	{
		return $"Hello from {nameof(InScriptGameObj)}";
	}
}

WriteLine("Creating Object");
var result = new InScriptGameObj();
WriteLine("Object created");

return result;