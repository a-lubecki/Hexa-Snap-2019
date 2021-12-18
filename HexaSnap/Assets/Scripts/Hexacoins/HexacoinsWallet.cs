/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

public class HexacoinsWallet : BaseModel {
	
	private static HexacoinsWalletListener to(BaseModelListener listener) {
		return (HexacoinsWalletListener) listener;
	}

    public int nbHexacoins { get; private set; }
    public int lastRemoteNbHexacoins { get; private set; }


    public HexacoinsWallet(int nbHexacoins, int lastRemoteNbHexacoins) {

        if (nbHexacoins < 0) {
            throw new ArgumentException();
        }
        if (lastRemoteNbHexacoins < 0) {
            throw new ArgumentException();
        }

        this.nbHexacoins = nbHexacoins;
        this.lastRemoteNbHexacoins = lastRemoteNbHexacoins;
	}

    public void updateHexacoins(int nb) {

        if (nb < 0) {
            throw new ArgumentException();
        }

        if (nb == nbHexacoins) {
            //ignore
            return;
        }

        nbHexacoins = nb;

        notifyListeners(listener => {
            to(listener).onNbHexacoinsChanged(this, 0);
        });
    }

    public void updateLastRemoteHexacoins(int remoteNbHexacoins) {

        if (remoteNbHexacoins < 0) {
            throw new ArgumentException();
        }

        lastRemoteNbHexacoins = remoteNbHexacoins;
    }

	public void addHexacoins(int nb) {

		if (nb < 0) {
			throw new ArgumentException();
		}
		if (nb <= 0) {
			//ignore
			return;
		}

		nbHexacoins += nb;

		notifyListeners(listener => {
			to(listener).onNbHexacoinsChanged(this, nb);
		});
	}

	public bool canPayHexacoins(int nb) {
		return (nb <= nbHexacoins);
	}

	public void payHexacoins(int nb) {

		if (nb < 0) {
			throw new ArgumentException();
		}
		if (nb <= 0) {
			//ignore
			return;
		}

		if (!canPayHexacoins(nb)) {
			nbHexacoins = 0;
		} else {
			nbHexacoins -= nb;
		}

		notifyListeners(listener => {
			to(listener).onNbHexacoinsChanged(this, -nb);
		});
	}

}

