using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Yarn.Unity
{
    [RequireComponent(typeof(DialogueRunner))]
    public class DialoguePortraitHandler : DialoguePresenterBase
    {
        [SerializeField] Image? characterSprite;
        [SerializeField] Image? characterShadow;

        [SerializeField] string portraitsResourcesPath = "Portraits";

        [SerializeField] float fadeDuration = 0.75f;

        private readonly Dictionary<string, Sprite> portraitsByName = new Dictionary<string, Sprite>(System.StringComparer.OrdinalIgnoreCase);
        private string? currentCharacterName;

        private void Awake()
        {
            var runner = GetComponent<DialogueRunner>();
            if (runner != null && !runner.DialoguePresenters.Contains(this))
            {
                runner.DialoguePresenters = runner.DialoguePresenters.Append(this);
            }
        }

        private void Start()
        {
            portraitsByName.Clear();
            foreach (var sprite in Resources.LoadAll<Sprite>(portraitsResourcesPath))
            {
                portraitsByName[sprite.name] = sprite;
            }

            SetPortraitAlpha(0f);
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            currentCharacterName = null;
            SetPortraitAlpha(0f);
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            currentCharacterName = null;
            SetPortraitAlpha(0f);
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            string? characterName = line.CharacterName;

            if (characterName == currentCharacterName)
            {
                return;
            }

            if (currentCharacterName != null)
            {
                await FadePortraitAsync(1f, 0f, token.HurryUpToken);
            }

            currentCharacterName = characterName;

            if (characterName != null && portraitsByName.TryGetValue(characterName, out var sprite))
            {
                SetPortraitSprite(sprite);
                await FadePortraitAsync(0f, 1f, token.HurryUpToken);
            }
        }

        private void SetPortraitSprite(Sprite? sprite)
        {
            if (characterSprite != null)
            {
                characterSprite.sprite = sprite;
            }
            if (characterShadow != null)
            {
                characterShadow.sprite = sprite;
            }
        }

        private void SetPortraitAlpha(float alpha)
        {
            if (characterSprite != null)
            {
                SetImageAlpha(characterSprite, alpha);
            }
            if (characterShadow != null)
            {
                SetImageAlpha(characterShadow, alpha);
            }
        }

        private static void SetImageAlpha(Image image, float alpha)
        {
            var colour = image.color;
            colour.a = alpha;
            image.color = colour;
        }

        private async YarnTask FadePortraitAsync(float from, float to, CancellationToken token)
        {
            SetPortraitAlpha(from);

            float accumulator = 0f;
            while (!token.IsCancellationRequested && accumulator < fadeDuration)
            {
                accumulator += Time.deltaTime;
                SetPortraitAlpha(Mathf.Lerp(from, to, accumulator / fadeDuration));
                await YarnTask.Yield();
            }

            SetPortraitAlpha(to);
        }
    }
}
