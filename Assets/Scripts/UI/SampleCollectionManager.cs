using System;
using System.Collections.Generic;
using UnityEngine;

public class SampleCollectionManager : MonoBehaviour
{
    [Header("Lists")]
    public CustomUICollectionManager collectionManager;
    public HashSet<SamplePackage> samples = new HashSet<SamplePackage>();
    public int sampleCount = 0;

    private void Awake()
    {
        
    }

    public void AddSample(string name, CustomDropdownOverrideSamplesManager sampleDropdown)
    {
        if (Contains(name, out var samplePackage))
        {
            samplePackage.sampleDropdown.Add(sampleDropdown);
            return;
        }

        samplePackage = new SamplePackage(name);
        samplePackage.Add(sampleDropdown);
        samplePackage.index = sampleCount;
        
        sampleDropdown.options2 = GetValidSamples(sampleCount);
        
        samples.Add(samplePackage);
        sampleCount++;
    }

    public List<string> GetValidSamples(int index)
    {
        List<string> samplesList = new List<string>();
        
        if (index == 0) return samplesList;
        
        foreach (SamplePackage sample in samples)
        {
            if (sample.index < index)
                samplesList.Add(sample.sampleName);
        }
        
        return samplesList;
    }
    
    public bool Contains(string name, out SamplePackage sample)
    {
        sample = null;
        
        foreach (SamplePackage s in samples)
        {
            if (s.sampleName == name)
            {
                sample = s;
                return true;
            }
        }

        return false;
    }
}

public class SamplePackage
{
    public string sampleName;
    public List<CustomDropdownOverrideSamplesManager> sampleDropdown;
    public int index;
    
    public SamplePackage(string name)
    {
        sampleName = name;
        sampleDropdown = new List<CustomDropdownOverrideSamplesManager>();
    }
    
    public void Add(CustomDropdownOverrideSamplesManager sd)
    {
        sampleDropdown.Add(sd);
    }

    public override bool Equals(object obj)
    {
        return obj is SamplePackage package &&
               sampleName == package.sampleName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(sampleName);
    }
}