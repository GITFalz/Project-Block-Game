public class CustomDoubleIntManager : CustomDoubleAbstractManager
{
    public override string ToCWorld()
    {
        int a = int.Parse(_fieldA.text);
        int b = int.Parse(_fieldB.text);
        
        string nameStr = doName ? name.text : "";

        return $"{nameStr} {a}, {b}\n";
    }
}