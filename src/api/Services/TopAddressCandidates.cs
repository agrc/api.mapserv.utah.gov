using System.Collections.Generic;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Services {
    public class TopAddressCandidates : TopNList<Candidate> {
        public TopAddressCandidates(int size, IComparer<Candidate> comparer)
            : base(ChooseSize(size), comparer) {
        }

        private static int ChooseSize(int size) {
            // allow score difference calculation for when no suggestion is selected
            if (size == 0) {
                return 2;
            }

            // otherwise suggestion size + 1 since one is the top candidate
            return size + 1;
        }
    }
}
