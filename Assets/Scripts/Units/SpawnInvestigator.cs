    using System;
    using UnityEngine;

    public class SpawnInvestigator : MonoBehaviour
    {
        public Investigator Spawn(Character character, int hitPoints)
        {
            Investigator investigator = character switch
            {
                Character.JennyBarnes => gameObject.AddComponent<JennyBarnes>(),
                Character.TrashcanPete => gameObject.AddComponent<TrashcanPete>(),
                Character.LilyChen => gameObject.AddComponent<LilyChen>(),
                Character.TommyMuldoon => gameObject.AddComponent<TommyMuldoon>(),
                Character.MichaelMcGlen => gameObject.AddComponent<MichaelMcGlen>(),
                _ => gameObject.AddComponent<Investigator>()
            };

            investigator.Initialize(hitPoints);
            
            Destroy(this);

            return investigator;
        }
    }