/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using UnityEngine;


//https://stackoverflow.com/questions/31389598/how-can-i-detect-a-shake-motion-on-a-mobile-device-using-unity3d-c-sharp
public class DeviceShakeDetector {


    private static readonly float MAX_FORCE = 3f;
    private static readonly float MAX_FORCE_BY_DIRECTION = MAX_FORCE * 0.5f;


    public static void detectShaking(float durationSec, Action completion) {
        
        if (!SpecificDeviceManager.Instance.isMobile()) {
            //not available on PC
            return;
        }

        Vector3 direction;
        if (!SpecificDeviceManager.Instance.isTablet()) {
            //phone
            direction = new Vector3(0, 1, 0);
        } else {
            //tablet
            direction = new Vector3(0, 0, 1);
        }

        Async.cancel("DeviceShakeDetector");
        Async.call(processDetectShaking(durationSec, direction, completion), "DeviceShakeDetector");
    }

    private static IEnumerator processDetectShaking(float durationSec, Vector3 wantedDirectionsPercentage, Action completion) {

        Vector3 lowPassValue = Vector3.zero;

        float totalTime = 0;

        while (totalTime <= durationSec) {

            yield return new WaitForSeconds(Constants.COROUTINE_FIXED_UPDATE_S);

            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, 0.01667f);
            Vector3 deltaAcceleration = acceleration - lowPassValue;

            if (deltaAcceleration.sqrMagnitude >= MAX_FORCE && 
                Mathf.Abs(deltaAcceleration.x) >= wantedDirectionsPercentage.x * MAX_FORCE_BY_DIRECTION &&
                Mathf.Abs(deltaAcceleration.y) >= wantedDirectionsPercentage.y * MAX_FORCE_BY_DIRECTION &&
                Mathf.Abs(deltaAcceleration.z) >= wantedDirectionsPercentage.z * MAX_FORCE_BY_DIRECTION) {

                //done
                completion();
                yield break;
            }
        }
    }


}