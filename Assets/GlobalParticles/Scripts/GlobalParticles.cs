using UnityEngine;
using System.Collections;
using Particle = UnityEngine.ParticleSystem.Particle;

namespace EbayVR
{
    [RequireComponent(typeof(ParticleSystem))]
    public class GlobalParticles : MonoBehaviour
    {
        // CACHED COMPONENTS
        private ParticleSystem _particleSystem;

        private Renderer particleRenderer;
        private static int colorPropId;
        
        [SerializeField]
        private Color32 lightParticleColor;
        [SerializeField]
        private Color32 darkParticleColor;
        private Color32 currentColor;

        [SerializeField]
        private float colorFadeDuration = 1f;

        #region MonoBehaviour Lifecycle
        protected void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            particleRenderer = GetComponent<Renderer>();
            colorPropId = Shader.PropertyToID("_TintColor");

            currentColor = particleRenderer.sharedMaterial.GetColor(colorPropId);
        }

        protected void OnEnable()
        {
            //AppManager.Instance.OnPhaseChanged += OnPhaseChanged;
        }
        protected void OnDisable()
        {
            //AppManager.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
        #endregion

        #region Events
        private void OnPhaseChanged(BasePhase nextPhase, BasePhase prevPhase)
        {
            /*
            if(prevPhase == null || nextPhase.BackgroundType != prevPhase.BackgroundType)
            {
                //ChangeParticleColor(nextPhase.BackgroundType);
                ChangeMaterialColor(nextPhase.BackgroundType);
            }
            */
        }
        #endregion

        /// <summary>
        /// Alternates the start color of this GameObject's particles between specified 
        /// light and dark colours depending on the background type.
        /// </summary>
        /// <param name="backgroundColor">Is the background light or dark?</param>
        /// <param name="isInstant">Should the colour change happen immediately?</param>
        public void ChangeParticleColor(BackgroundType backgroundColor, bool isInstant = false)
        {
            Particle[] particles = new Particle[_particleSystem.particleCount];
            _particleSystem.GetParticles(particles);

            if(particles.Length > 0)
            {
                Color32 oldColor = particles[0].startColor;
                Color32 newColor = backgroundColor == BackgroundType.LIGHT ? lightParticleColor : darkParticleColor;

                if (isInstant)
                {
                    for (int i = 0; i < particles.Length; i++)
                        particles[i].startColor = newColor;
                    
                    _particleSystem.SetParticles(particles, particles.Length);
                }
                else
                {
                    StartCoroutine(CR_LerpParticleColor(particles, oldColor, newColor, colorFadeDuration));
                }
            }
        }

        private IEnumerator CR_LerpParticleColor(Particle[] particles, Color32 from, Color32 to, float duration)
        {
            float speed = 1f / duration;

            for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
            {
                for(int i = 0; i < particles.Length; i++)
                    particles[i].startColor = Color32.Lerp(from, to, t);
                
                _particleSystem.SetParticles(particles, particles.Length);

                yield return null;
            }
        }

        /// <summary>
        /// Changes the main color on this GameObject's ParticleSystem renderer between
        /// specified light and dark colours depending on the background type.
        /// </summary>
        /// <param name="backgroundColor">Is the background light or dark?</param>
        /// <param name="isInstant">Should the colour change happen immediately?</param>
        public void ChangeMaterialColor(BackgroundType backgroundType, bool isInstant = false)
        {
            Color32 newColor = backgroundType == BackgroundType.LIGHT ? lightParticleColor : darkParticleColor;

            if (isInstant)
            {
                particleRenderer.sharedMaterial.SetColor(colorPropId, newColor);
                currentColor = newColor;
            }
            else
            {
                StartCoroutine(CR_LerpMaterialColor(currentColor, newColor, colorFadeDuration));
            }
        }
        
        private IEnumerator CR_LerpMaterialColor(Color32 from, Color32 to, float duration)
        {
            float speed = 1f / duration;

            for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
            {
                particleRenderer.sharedMaterial.SetColor(colorPropId, Color32.Lerp(from, to, t));
                currentColor = to;

                yield return null;
            }

            particleRenderer.sharedMaterial.SetColor(colorPropId, Color32.Lerp(from, to, 1f));
            currentColor = to;
        }
    }
}