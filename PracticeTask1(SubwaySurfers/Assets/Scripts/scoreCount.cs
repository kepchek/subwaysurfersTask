using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class scoreCount : MonoBehaviour
{
    public Text score;
    long scoreint = 0;
    private void Start() {
        score.text = "0";
    }
    private void FixedUpdate() {
        scoreint++;
        score.text = Convert.ToString(scoreint);
    }
}
