using System;
using System.Collections.Generic;
using System.Data;

namespace pmis.reviewinfo
{
    public interface IReviewInfoDao
    {
        DataTable LoadReviewInfo(RegisterDocument doc);

        void DeleteReviewInfo();

        void ImportReviewInfoData(List<ReviewInfo> docs, Action<ReviewInfo> progressCallback);

        int LoadReviewInfoCount();
    }
}
