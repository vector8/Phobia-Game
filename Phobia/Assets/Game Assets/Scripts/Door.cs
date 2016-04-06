using UnityEngine;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class Door : Interactable
{
    public float MAX_TIME;
    public Transform rotatePoint;
    public float openAngle = 90.0f;
    public Vector3 rotateAxis = new Vector3(0f, 1f, 0f);

    private bool open = false;
    private float timer = 0.0f;
    private bool activated = false;
    private AudioSource audio;

    protected override void Start()
    {
        base.Start();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            timer += Time.deltaTime;
            float u = timer / MAX_TIME;
            if (u >= 1f)
            {
                u = 1f;
                activated = false;
                timer = 0f;
            }

            float angle;
            if (open)
            {
                angle = Mathf.Lerp(0, openAngle, u);
            }
            else
            {
                angle = Mathf.Lerp(openAngle, 0, u);
            }

            //Vector3 euler = rotatePoint.localEulerAngles;
            //euler.y = yAngle;
            rotatePoint.localEulerAngles = angle * rotateAxis;
        }
    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        if (audio != null)
        {
            audio.Play();
        }

        if (open)
        {
            timer = 0.0f;
            activated = true;
            open = false;
        }
        else
        {
            timer = 0.0f;
            activated = true;
            open = true;
        }
    }
}
