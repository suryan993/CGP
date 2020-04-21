using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedInfo : MonoBehaviour
{
    // public values
    // slider object for speed display
    public Slider slider;

    // range values for displaying the bars properly on the speed display
    public float topRangeValue;
    public float bottomRangeValue;

    // default constant values for display
    const float DEFAULT_SLIDER_HEIGHT = 160f;
    const float DEFAULT_BOTTOM_VALUE = -80f;

    // actual bar objects used on the canvas
    [SerializeField]
    GameObject topRange;
    [SerializeField]
    GameObject bottomRange;
    // Start is called before the first frame update
    void Awake()
    {
        // set the slider to the component part
        //slider = GetComponent<Slider>();
        slider = GameObject.FindGameObjectWithTag("SpeedSlider").GetComponent<Slider>();

        // set the range values to their defaults
        topRangeValue = DEFAULT_BOTTOM_VALUE;
        bottomRangeValue = DEFAULT_BOTTOM_VALUE;
    }

    // Sets slider value to display the current velocity of the player
    public void DisplayCurrentSpeed(float currentV, float minV, float maxV)
    {
        slider.value = (currentV - minV) / maxV;
    }

    // Takes in values and maps them to the range based on the height of the rect transform of the slider
    public void MapToRange(float bottomValue, float topValue)
    {
        // finds the heights of the bottom and top parts of the range 
        float lowVal = DEFAULT_SLIDER_HEIGHT * bottomValue;
        float highVal = DEFAULT_SLIDER_HEIGHT * topValue;

        // sets the heights used to for the bars in the display
        topRangeValue = DEFAULT_BOTTOM_VALUE + lowVal;
        bottomRangeValue = DEFAULT_BOTTOM_VALUE + highVal;
    }

    // Uses top and bottom range values to set the heights of the top and bottom bar objects
    public void SetRange()
    {
        topRange.GetComponent<RectTransform>().localPosition = new Vector3(topRangeValue, -25f, 0f);
        bottomRange.GetComponent<RectTransform>().localPosition = new Vector3(bottomRangeValue, -25f, 0f);
    }
}
