/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface AxisListener : BaseModelListener {

    void onRotationAngleChange(Axis axis, float angleDegrees);

    void onRotationForceClockwiseChange(Axis axis, float force);

    void onRotationForceCounterClockwiseChange(Axis axis, float force);

}

