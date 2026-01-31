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
        moustache = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Moustache"));
        eyebrows = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Eyebrows"));
        noses = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Nose"));
    }

    Image FindImageByName(string objectName)
    {
        var go = GameObject.Find(objectName);
        return go != null ? go.GetComponent<Image>() : null;
    }

    public void GenerateFace(int? seed)
    {
        Init();

        //var previousState = Random.state;
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
        //beardRenderer.sprite = selectedBeard;
        //moustacheRenderer.sprite = selectedMoustache;
        eyebrowsRenderer.sprite = selectedEyebrows;
        noseRenderer.sprite = selectedNose;

        //Random.state = previousState;
    }
}
