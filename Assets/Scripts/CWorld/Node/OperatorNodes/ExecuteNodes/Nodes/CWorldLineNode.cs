using System.Collections.Generic;

public class CWorldLineNode
{
    public List<CWorldLineConditionNode> Conditions;
    
    public CWorldLineNode()
    {
        Conditions = new List<CWorldLineConditionNode>();
    }

    public void GenerateLine()
    {
        
    }
}

public class CWorldLineConditionNode
{
    public bool IsTrue()
    {
        
    }
}