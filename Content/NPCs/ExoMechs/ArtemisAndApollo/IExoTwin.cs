﻿namespace DifferentExoMechs.Content.NPCs.Bosses
{
    public interface IExoTwin
    {
        /// <summary>
        /// The current frame for this Exo Twin.
        /// </summary>
        public int Frame
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this Exo Twin has fully entered its second phase yet or not.
        /// </summary>
        /// 
        /// <remarks>
        /// In this context, "second phase" does not refer to the phases of the overall battle, instead referring to whether the Exo Twin has removed its lens and revealed its full mechanical form.
        /// </remarks>
        public bool InPhase2
        {
            get;
            set;
        }

        /// <summary>
        /// The current animation of this Exo Twin.
        /// </summary>
        public ExoTwinAnimation Animation
        {
            get;
            set;
        }
    }
}