using ugrc.api.Models.ArcGis;

namespace ugrc.api.Services;
public class TopAddressCandidates(int size, IComparer<Candidate> comparer) : TopNList<Candidate>(ChooseSize(size), comparer) {
    private static int ChooseSize(int size) {
        // allow score difference calculation for when no suggestion is selected
        if (size == 0) {
            return 2;
        }

        // otherwise suggestion size + 1 since one is the top candidate
        return size + 1;
    }
}
