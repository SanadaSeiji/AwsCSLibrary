using AwsCSLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AwsCSLibraryTests
{
    public partial class AwsManagersTests
    {
        [TestMethod]
        public async Task GetLinks_GivenWantedNode_FromDynamoDb()
        {
            string filterColumn = "SourceProject"; // "Source/UUID/Target" are reserved by AWS unusable as filter
            string searchedNode = "test";

            var list = await aws.GetRecordsAsync(filterColumn, searchedNode);
            Console.WriteLine(list.Count);
        }

        [TestMethod]
        public async Task InsertLink_InDynamoDb_then_Delete()
        {
            string source = "sourceNode";
            string target = "targetNode";

            await aws.PutRecordAsync(source, target);


            var obj = new DBRecord()
            {
                Id = "000",
                Source = "sourceNode",
                Target = "targetNode"
            };
            await aws.DeleteRecordAsync(obj.Id);// when item does not exist, do not throw exception
        }
    }
}
