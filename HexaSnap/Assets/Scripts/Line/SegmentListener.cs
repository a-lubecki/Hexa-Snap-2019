/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface SegmentListener : BaseModelListener {

	void onAdvancePercentageUpdate(Segment segment);

    void onSegmentColorChange(Segment segment);

}


