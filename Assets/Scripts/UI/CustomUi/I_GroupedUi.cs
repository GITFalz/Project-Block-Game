using System.Collections.Generic;

public interface I_GroupedUi
{
    void ChangeSampleName(CustomNodeGroupManager nodeGroupManager);
    void AlignCollections();
    bool DoHorizontalSpacing();
    void UpdateCollection(string name, IACustomModifier modifier);
}