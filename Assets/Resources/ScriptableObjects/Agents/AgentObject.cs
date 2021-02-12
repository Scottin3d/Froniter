using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentGender {
    male,
    female,
}
[CreateAssetMenu(fileName = "New Agent Object", menuName = "Create Agent")]
public class AgentObject : ScriptableObject {
    private bool init = false;

    public AgentGender gender;
    private string firstName;
    private string lastName;
    public float agentMaxEnergy;
    public float energyIncriment;
    public float rechargeRate;

    public string FirstName { get => firstName; set => firstName = value; }
    public string LastName { get => lastName; set => lastName = value; }

    public void InitObject() {
        if (!init) {
            FirstName = (gender == AgentGender.male) ? RandomName.current.GetRandomMaleName() : RandomName.current.GetRandomFemaleName();
            LastName = RandomName.current.GetRandomPlaceName();
            init = true;
            }
    }
}
