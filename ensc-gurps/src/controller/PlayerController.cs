using ensc_gurps.model.character;
using System;
using System.Collections.Generic;

namespace ensc_gurps.controller
{
    public class PlayerController
    {
        public Character Player { get; private set; }

        public List<Skill> Skills { get { return Player.Skills; } }

        private ClassInfluence _classInfluence;

        private List<string> SkillsAvailable
        {
            get { return _classInfluence.AvailableSkills; }
        }

        private List<TraitModifier> TraitModifiers
        {
            get { return _classInfluence.TraitModifiers; }
        }

        public PlayerController(Character player, ClassInfluence classInfluence)
        {
            Player = player;
            _classInfluence = classInfluence;
        }

        public bool ChallengeTrait(int jet, int goal, string traitID)
        {
            return Player.GetTrait(traitID).Value + jet > goal; 
        }

        public bool IsNPC()
        {
            return Player is NPC;
        }

        public void FightAgainst(PlayerController ennemy)
        {
            List<Skill> listSkill = Player.Skills;

            int input;
            if (!this.IsNPC())
            {
                Console.WriteLine();
                int i;
                for (i = 0; i < listSkill.Count; i++)
                    Console.WriteLine(" " + i + ". " + listSkill[i].Name);

                Console.WriteLine(" " + i + ". Attaquer à main nue");

                Console.Write("\n C'est votre tour que faites vous ? ");

                if (!int.TryParse(Console.ReadLine(), out input))
                    input = listSkill.Count;
            }
            else
            {
                Console.WriteLine("\n " + Player.Name + " joue.");
                if (listSkill.Count == 0)
                    input = listSkill.Count;
                else
                    input = new Random().Next() % listSkill.Count;
            }

            ennemy.IncreaseHP(-1 * Player.GetStrike()); // main nue dans tous les cas

            if(input < listSkill.Count) // si un skill est utilisé en plus
            {            
                ennemy.IncreaseHP(-1 * listSkill[input].StrikeValue);
                this.IncreaseHP(listSkill[input].HealValue);
            }
        }

        public bool TryToLearn(Skill s)
        {
            if (SkillsAvailable.Contains(s.Name))
            {
                Player.Learn(s);
                return true;
            }
            else
            {
                return false;
            }
        }

        private float GetProgressionFactor(string trait)
        {
            foreach (TraitModifier t in TraitModifiers)
                if (t.TraitID == trait)
                    return t.Factor;
            return 1.0f;
        }

        private void IncreaseHP(float value)
        {
            Player.Status[0].Value += value;
        }

        public float GetHP()
        {
            return Player.Status[0].Value;
        }

        public bool IsAlive()
        {
            return Player.Status[0].Value > 0.01;
        }

        public void LevelUp()
        {
            foreach (Trait t in Player.Traits)
            {
                t.Value += 1 * GetProgressionFactor(t.TraitID);
            }
        }
    }
}
