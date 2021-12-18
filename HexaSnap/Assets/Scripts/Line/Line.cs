/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;

/**
 * 
 * % bevel (float) / start vertical first (bool)
 *
 * case 0
 * o---o   o     o   o
 *         |    /     \
 *         o   o       o
 *
 *
 * case 1 : 0 / true
 * .---------o
 * |
 * |
 * |
 * o
 *
 *
 * case 2 : 0.5 / true
 *   .-------o
 *  /
 * °
 * |
 * o
 *
 *
 * case 3 : 1 / true
 *     .-----o      o
 *    /            /
 *   /            °
 *  /             |
 * o              o
 * 
 *
 * case 4 : 1 / false
 *           o      o
 *          /       |
 *         /        °
 *        /        /
 * o-----°        o
 *
 *
 * case 5 : 0.5 / false
 *           o
 *           |
 *           °
 *          /
 * o-------°
 *
 *
 * case 6 : 0 / false
 *           o
 *           |
 *           |
 *           |
 * o---------°
 *
 */
public class Line : BaseModel {


    private Segment[] segments;

    private float advancePercentage = 0;
    public readonly float totalDistance;

    public Color color { get; private set; }

    public float percentageBevel { get; private set; }
    public bool mustStartVerticalFirst { get; private set; }


    public Line(Vector3 posBegin, Vector3 posEnd, SegmentThickness thickness, float percentageBevel, bool mustStartVerticalFirst) {

		if (percentageBevel < 0 || percentageBevel > 1) {
			throw new ArgumentException();
		}

        this.percentageBevel = percentageBevel;
        this.mustStartVerticalFirst = mustStartVerticalFirst;

        Vector3 begin = new Vector3(posBegin.x, posBegin.y, Constants.Z_POS_LINES);
		Vector3 end = new Vector3(posEnd.x, posEnd.y, Constants.Z_POS_LINES);

		if (begin.Equals(end)) {
			throw new NotSupportedException("Can't draw a line with only one point");
		}


		//fill segments array with formula (only 1, 2 or 3 segments depending of the markers alignment and the percentages in parameters)

		float xBegin = begin.x;
		float yBegin = begin.y;
		float xEnd = end.x;
		float yEnd = end.y;

		float dx = Mathf.Abs(xBegin - xEnd);
		float dy = Mathf.Abs(yBegin - yEnd);

		if (xBegin == xEnd || yBegin == yEnd || 
			(dx == dy && percentageBevel >= 1)) {

			//markers are aligned horizontally, vertically or diagonally : 1 segment
			//case 0
			segments = new Segment[1];
			segments[0] = new Segment(begin, end, thickness);

		} else {

			if (percentageBevel <= 0) {

				//no bevel : 2 segments, vertical + horizontal
				segments = new Segment[2];

				Vector3 p1;

				if (mustStartVerticalFirst) {
					//case 1
					p1 = new Vector3(xBegin, yEnd, Constants.Z_POS_LINES);

				} else {
					//case 6 : horizontal then vertical
					p1 = new Vector3(yBegin, xEnd, Constants.Z_POS_LINES);
				}

				segments[0] = new Segment(begin, p1, thickness);
				segments[1] = new Segment(p1, end, thickness);

			} else {

				float dmin = dx < dy ? dx : dy;

				float xMultiplier = (xBegin < xEnd) ? 1 : -1;
				float yMultiplier = (yBegin < yEnd) ? 1 : -1;

				if (percentageBevel < 1) {

					//bevel but not max : 3 segments, 1 diagonal + 1 horizontal + 1 vertical
					segments = new Segment[3];

					float dBevel = dmin * percentageBevel;

					Vector3 p1;
					Vector3 p2;

					if (mustStartVerticalFirst) {
						//case 2
						p1 = new Vector3(xBegin, yEnd - yMultiplier * dBevel, Constants.Z_POS_LINES);
						p2 = new Vector3(xBegin + xMultiplier * dBevel, yEnd, Constants.Z_POS_LINES);

					} else {
						//case 5
						p1 = new Vector3(xEnd - xMultiplier * dBevel, yBegin, Constants.Z_POS_LINES);
						p2 = new Vector3(xEnd, yBegin + yMultiplier * dBevel, Constants.Z_POS_LINES);
					}

					segments[0] = new Segment(begin, p1, thickness);
					segments[1] = new Segment(p1, p2, thickness);
					segments[2] = new Segment(p2, end, thickness);

				} else {

					//max bevel : 2 segments, diagonal + orthogonal
					segments = new Segment[2];

					Vector3 p1;

					if (mustStartVerticalFirst) {
						//case 3
						if (dx > dy) {
							//diagonal then horizontal
							p1 = new Vector3(xBegin + xMultiplier * dmin, yEnd, Constants.Z_POS_LINES);

						} else {
							//vertical then diagonal
							p1 = new Vector3(xBegin, yEnd - yMultiplier * dmin, Constants.Z_POS_LINES);
						}

					} else {
						//case 4
						if (dx > dy) {
							//horizontal then diagonal
							p1 = new Vector3(xEnd - xMultiplier * dmin, yBegin, Constants.Z_POS_LINES);

						} else {
							//diagonal then vertical
							p1 = new Vector3(xEnd, yBegin + yMultiplier * dmin, Constants.Z_POS_LINES);
						}

					}

					segments[0] = new Segment(begin, p1, thickness);
					segments[1] = new Segment(p1, end, thickness);

				}

			}
		}

		//find the total distance
		foreach (Segment s in segments) {
			totalDistance += s.totalDistance;
		}


        setColor(Constants.COLOR_LINE_DEFAULT);

	}

	public float getAdvancePercentage() {
		return advancePercentage;
	}

	public Vector3 getBeginPosition() {
		return segments[0].posBegin;
	}

	public Vector3 getEndPosition() {
		return segments[segments.Length - 1].posEnd;
	}

	public int getNbSegments() {
		return segments.Length;
	}

	public Segment getSegment(int pos) {
		return segments[pos];
	}


	public void updateAdvancePercentage(float advancePercentage) {

		if (advancePercentage < 0) {
            advancePercentage = 0;
		} else if (advancePercentage > 1) {
            advancePercentage = 1;
		}

		if (advancePercentage == this.advancePercentage) {
			return;
		}

		this.advancePercentage = advancePercentage;

		float distanceToReach = totalDistance * advancePercentage;
		float elapsedDistance = 0;
		bool isReached = (advancePercentage <= 0);

		//update all the segments
		foreach (Segment s in segments) {

			if (isReached) {
				s.updateAdvancePercentage(0);
				continue;
			}

			float ds = s.totalDistance;

			if (elapsedDistance + ds <= distanceToReach) {

				//all the segment distance is reached
				s.updateAdvancePercentage(1);

			} else {

                //the distance is reached but the segment must be updated with the correct percentage
                s.updateAdvancePercentage((distanceToReach - elapsedDistance) / ds);

				isReached = true;
			}

			elapsedDistance += ds;
		}
	}

    public void setColor(Color color) {

        if (this.color == color) {
            return;
        }

        this.color = color;

        foreach (Segment s in segments) {
            s.setColor(color);
        }
    }

}
