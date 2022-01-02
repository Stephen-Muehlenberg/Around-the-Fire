using UnityEngine;

public class Adventurer
{
  public string name;
  public Sprite icon;

  public int hunger;
  public int rest;
  public int mood;
  public int health;

  public CampAction action;
  public AdventurerPortrait portrait;

  public enum Stat
  {
    HEALTH, HUNGER, REST, MORALE
  }
}