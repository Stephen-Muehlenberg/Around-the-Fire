using System.Collections.Generic;

/// <summary>
/// A location <see cref="Portrait"/>s can be assigned to.
/// Visually represented by a <see cref="PortraitZoneUiGroup"/>.
/// </summary>
public class PortraitZone
{
  public string name;
  public List<Portrait> portraits = new();
  public int maxPortraits;
  public bool hasSpace => portraits.Count < maxPortraits;
}
