using UnityEngine;
using UnityEngine.Windows.Speech;
using System;

namespace HTW.CAVE.Interaction
{
    public class KeywordRecognition : MonoBehaviour
    {
        [SerializeField]
        private string[] m_Keywords;

        private KeywordRecognizer m_Recognizer;

        // Event to be invoked when a phrase is recognized
        public event Action<PhraseRecognizedEventArgs> OnKeywordRecognized;

        private void Start()
        {
            m_Recognizer = new(m_Keywords);
            m_Recognizer.OnPhraseRecognized += HandlePhraseRecognized;
            m_Recognizer.Start();
        }

        private void HandlePhraseRecognized(PhraseRecognizedEventArgs args)
        {
            Debug.Log($"phrase recognized {args.text}");

            // Invoke the event, passing the recognized phrase
            OnKeywordRecognized?.Invoke(args);
        }
    }
}