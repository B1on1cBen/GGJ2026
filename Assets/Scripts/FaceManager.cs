using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceManager : MonoBehaviour
{
    [Header("Loaded Face Parts")]
    [SerializeField] private static List<Sprite> eyes = new List<Sprite>();
    [SerializeField] private static List<Sprite> mouths = new List<Sprite>();
    [SerializeField] private static List<Sprite> ears = new List<Sprite>();
    [SerializeField] private static List<Sprite> hair = new List<Sprite>();
    [SerializeField] private static List<Sprite> accessories = new List<Sprite>();
    [SerializeField] private static List<Sprite> eyebrows = new List<Sprite>();
    [SerializeField] private static List<Sprite> noses = new List<Sprite>();
    [SerializeField] private static List<Sprite> beard = new List<Sprite>();
    [SerializeField] private static List<Sprite> moustache = new List<Sprite>();
    [SerializeField] private static Sprite empty;

    [Header("Selected Face Parts")]
    [SerializeField] private Sprite selectedEyes;
    [SerializeField] private Sprite selectedMouth;
    [SerializeField] private Sprite selectedEars;
    [SerializeField] private Sprite selectedHair;
    [SerializeField] private Sprite selectedAccessory;
    [SerializeField] private Sprite selectedBeard;
    [SerializeField] private Sprite selectedMoustache;
    [SerializeField] private Sprite selectedEyebrows;
    [SerializeField] private Sprite selectedNose;
    
    [Header("Face Part Renderers")]
    [SerializeField] private Image eyesRenderer;
    [SerializeField] private Image mouthRenderer;
    [SerializeField] private Image earsRenderer;
    [SerializeField] private Image hairRenderer;
    [SerializeField] private Image accessoryRenderer;
    [SerializeField] private Image beardRenderer;
    [SerializeField] private Image moustacheRenderer;
    [SerializeField] private Image eyebrowsRenderer;
    [SerializeField] private Image noseRenderer;

    [Header("Face Seed")]
    [SerializeField] private int faceSeed;
    public int FaceSeed => faceSeed;
    
    private static readonly float accessoryEmptyChance = 0.50f;
    private static readonly float beardEmptyChance     = 0.45f;
    private static readonly float hairEmptyChance      = 0.24f;
    private static readonly float moustacheEmptyChance = 0.42f;

    static bool init = false;
    
    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (init) return;
        init = true;

        eyes = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Eyes"));
        mouths = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Mouth"));
        ears = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Ears"));
        hair = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Hair"));
        accessories = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Accessories"));
        beard = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Beard"));
        moustache = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Mustache"));
        eyebrows = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Eyebrows"));
        noses = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Nose"));
        empty = Resources.Load<Sprite>("Faces/Faces-EMPTY");
    }
    
    private static Sprite GetRandomFromList(List<Sprite> sprites)
    {
        if (sprites == null || sprites.Count <= 0) return null;
        return sprites[Random.Range(0, sprites.Count)];
    }
    
    private static Sprite GetRandomOrEmpty(List<Sprite> sprites, Sprite emptySprite, float emptyChance)
    {
        if (Random.value < Mathf.Clamp01(emptyChance)) return emptySprite;
        return GetRandomFromList(sprites);
    }

    public void GenerateFace(int? seed)
    {
        Init();

        if (seed.HasValue)
        {
            faceSeed = seed.Value;
            Random.InitState(faceSeed);
        }
        else
        {
            faceSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        selectedEyes = eyes.Count > 0 ? eyes[Random.Range(0, eyes.Count)] : null;
        selectedMouth = mouths.Count > 0 ? mouths[Random.Range(0, mouths.Count)] : null;
        selectedEars = ears.Count > 0 ? ears[Random.Range(0, ears.Count)] : null;
        selectedHair = hair.Count > 0 ? hair[Random.Range(0, hair.Count)] : null;
        selectedAccessory = accessories.Count > 0 ? accessories[Random.Range(0, accessories.Count)] : null;
        selectedBeard = beard.Count > 0 ? beard[Random.Range(0, beard.Count)] : null;
        selectedMoustache = moustache.Count > 0 ? moustache[Random.Range(0, moustache.Count)] : null;
        selectedEyebrows = eyebrows.Count > 0 ? eyebrows[Random.Range(0, eyebrows.Count)] : null;
        selectedNose = noses.Count > 0 ? noses[Random.Range(0, noses.Count)] : null;

        eyesRenderer.sprite = selectedEyes;
        mouthRenderer.sprite = selectedMouth;
        earsRenderer.sprite = selectedEars;
        hairRenderer.sprite = selectedHair;
        accessoryRenderer.sprite = selectedAccessory;
        beardRenderer.sprite = selectedBeard;
        moustacheRenderer.sprite = selectedMoustache;
        eyebrowsRenderer.sprite = selectedEyebrows;
        noseRenderer.sprite = selectedNose;
    }

    public void RandomizeFeatureByIndex(int featureIndex)
    {
        Init();
        
        switch (featureIndex)
        {
            case 0:
                selectedEyes = GetRandomFromList(eyes);
                eyesRenderer.sprite = selectedEyes;
                break;
            case 1:
                selectedMouth = GetRandomFromList(mouths);
                mouthRenderer.sprite = selectedMouth;
                break;
            case 2:
                selectedEars = GetRandomFromList(ears);
                earsRenderer.sprite = selectedEars;
                break;
            case 3:
                selectedHair = GetRandomOrEmpty(hair, empty, hairEmptyChance);
                hairRenderer.sprite = selectedHair;
                break;
            case 4:
                selectedAccessory = GetRandomOrEmpty(accessories, empty, accessoryEmptyChance);
                accessoryRenderer.sprite = selectedAccessory;
                break;
            case 5:
                selectedBeard = GetRandomOrEmpty(beard, empty, beardEmptyChance);
                beardRenderer.sprite = selectedBeard;
                break;
            case 6:
                selectedMoustache = GetRandomOrEmpty(moustache, empty, moustacheEmptyChance);
                moustacheRenderer.sprite = selectedMoustache;
                break;
            case 7:
                selectedEyebrows = GetRandomFromList(eyebrows);
                eyebrowsRenderer.sprite = selectedEyebrows;
                break;
            case 8:
                selectedNose = GetRandomFromList(noses);
                noseRenderer.sprite = selectedNose;
                break;
        }
    }

    public struct FaceParts
    {
        public Sprite Eyes;
        public Sprite Mouth;
        public Sprite Ears;
        public Sprite Hair;
        public Sprite Accessory;
        public Sprite Beard;
        public Sprite Moustache;
        public Sprite Eyebrows;
        public Sprite Nose;
    }

    public FaceParts GetFaceParts()
    {
        return new FaceParts
        {
            Eyes = selectedEyes,
            Mouth = selectedMouth,
            Ears = selectedEars,
            Hair = selectedHair,
            Accessory = selectedAccessory,
            Beard = selectedBeard,
            Moustache = selectedMoustache,
            Eyebrows = selectedEyebrows,
            Nose = selectedNose
        };
    }

    public void ApplyFaceParts(FaceParts parts)
    {
        selectedEyes = parts.Eyes;
        selectedMouth = parts.Mouth;
        selectedEars = parts.Ears;
        selectedHair = parts.Hair;
        selectedAccessory = parts.Accessory;
        selectedBeard = parts.Beard;
        selectedMoustache = parts.Moustache;
        selectedEyebrows = parts.Eyebrows;
        selectedNose = parts.Nose;

        eyesRenderer.sprite = selectedEyes;
        mouthRenderer.sprite = selectedMouth;
        earsRenderer.sprite = selectedEars;
        hairRenderer.sprite = selectedHair;
        accessoryRenderer.sprite = selectedAccessory;
        beardRenderer.sprite = selectedBeard;
        moustacheRenderer.sprite = selectedMoustache;
        eyebrowsRenderer.sprite = selectedEyebrows;
        noseRenderer.sprite = selectedNose;
    }
}
