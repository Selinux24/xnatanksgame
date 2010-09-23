using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace GameComponents.Sound
{
    using Physics;

    /// <summary>
    /// Audio manager keeps track of what 3D sounds are playing, updating
    /// their settings as the camera and entities move around the world,
    /// and automatically disposing cue instances after they finish playing.
    /// </summary>
    public class SoundComponent : GameComponent
    {
        private AudioEngine m_AudioEngine;
        private SoundBank m_SoundBank;

        /// <summary>
        /// The listener describes the ear which is hearing 3D sounds. This is usually set to match the camera.
        /// </summary>
        public AudioListener Listener { get; protected set; }
        /// <summary>
        /// The emitter describes an entity which is making a 3D sound.
        /// </summary>
        private AudioEmitter m_Emitter = new AudioEmitter();
        /// <summary>
        /// Keep track of all the 3D sounds that are currently playing.
        /// </summary>
        private List<Cue3D> m_ActiveCues = new List<Cue3D>();
        /// <summary>
        /// Keep track of spare Cue3D instances, so we can reuse them.
        /// Otherwise we would have to allocate new instances each time
        /// a sound was played, which would create unnecessary garbage.
        /// </summary>
        private Stack<Cue3D> m_CuePool = new Stack<Cue3D>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public SoundComponent(Game game)
            : base(game)
        {
            this.Listener = new AudioListener();
        }
        /// <summary>
        /// Loads the XACT data.
        /// </summary>
        public override void Initialize()
        {
            this.m_AudioEngine = new AudioEngine("Content/audio.xgs");
            this.m_SoundBank = new SoundBank(this.m_AudioEngine, "Content/Sound Bank.xsb");

            base.Initialize();
        }
        /// <summary>
        /// Unloads the XACT data.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.m_SoundBank.Dispose();
                    this.m_AudioEngine.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        /// <summary>
        /// Updates the state of the 3D audio system.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Loop over all the currently playing 3D sounds.
            int index = 0;

            while (index < this.m_ActiveCues.Count)
            {
                Cue3D cue3D = this.m_ActiveCues[index];

                if (cue3D.Cue.IsStopped)
                {
                    // If the cue has stopped playing, dispose it.
                    cue3D.Cue.Dispose();

                    // Store the Cue3D instance for future reuse.
                    this.m_CuePool.Push(cue3D);

                    // Remove it from the active list.
                    this.m_ActiveCues.RemoveAt(index);
                }
                else
                {
                    // If the cue is still playing, update its 3D settings.
                    Apply3D(cue3D);

                    index++;
                }
            }

            // Update the XACT engine.
            this.m_AudioEngine.Update();

            base.Update(gameTime);
        }
        /// <summary>
        /// Triggers a new 3D sound.
        /// </summary>
        public Cue Play3DCue(string cueName, IPhysicObject emitter)
        {
            Cue3D cue3D;

            if (this.m_CuePool.Count > 0)
            {
                // If possible, reuse an existing Cue3D instance.
                cue3D = this.m_CuePool.Pop();
            }
            else
            {
                // Otherwise we have to allocate a new one.
                cue3D = new Cue3D();
            }

            // Fill in the cue and emitter fields.
            cue3D.Cue = this.m_SoundBank.GetCue(cueName);
            cue3D.Emitter = emitter;

            // Set the 3D position of this cue, and then play it.
            Apply3D(cue3D);

            cue3D.Cue.Play();

            // Remember that this cue is now active.
            this.m_ActiveCues.Add(cue3D);

            return cue3D.Cue;
        }
        /// <summary>
        /// Updates the position and velocity settings of a 3D cue.
        /// </summary>
        private void Apply3D(Cue3D cue3D)
        {
            this.m_Emitter.Position = cue3D.Emitter.Position;
            this.m_Emitter.Forward = Vector3.Forward;
            this.m_Emitter.Up = Vector3.Up;
            this.m_Emitter.Velocity = Vector3.Zero;

            cue3D.Cue.Apply3D(this.Listener, this.m_Emitter);
        }

        /// <summary>
        /// Internal helper class for keeping track of an active 3D cue,
        /// and remembering which emitter object it is attached to.
        /// </summary>
        private class Cue3D
        {
            public Cue Cue;
            public IPhysicObject Emitter;
        }
    }
}
