using GitTools.Testing;
using GitVersion.Core.Tests.Helpers;
using NUnit.Framework;

namespace GitVersion.Core.Tests.IntegrationTests
{
    [TestFixture]
    public class GitflowScenarios : TestBase
    {
        [Test]
        public void GitflowComplexExample()
        {
            const string developBranch = "develop";
            const string feature1Branch = "feature/f1";
            const string feature2Branch = "feature/f2";
            const string release1Branch = "release/1.1.0";
            const string release2Branch = "release/1.2.0";
            const string hotfixBranch = "hotfix/hf";

            using var fixture = new BaseGitFlowRepositoryFixture("1.0.0");
            fixture.AssertFullSemver("1.1.0-alpha.1");

            // Feature 1
            fixture.BranchTo(feature1Branch);
            fixture.MakeACommit("added feature 1");
            fixture.AssertFullSemver("1.1.0-f1.1+2");
            fixture.Checkout(developBranch);
            fixture.MergeNoFF(feature1Branch);
            fixture.Repository.Branches.Remove(fixture.Repository.Branches[feature1Branch]);
            fixture.AssertFullSemver("1.1.0-alpha.3");

            // Release 1.1.0
            fixture.BranchTo(release1Branch);
            fixture.MakeACommit("release stabilization");
            fixture.AssertFullSemver("1.1.0-beta.1+1");
            fixture.Checkout(MainBranch);
            fixture.MergeNoFF(release1Branch);
            fixture.AssertFullSemver("1.1.0+0"); // why +0 and not +5??
            fixture.AssertFullSemver("1.0.1+5", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());
            fixture.ApplyTag("1.1.0");
            fixture.AssertFullSemver("1.1.0");
            fixture.Checkout(developBranch);
            fixture.MergeNoFF(release1Branch);
            fixture.Repository.Branches.Remove(fixture.Repository.Branches[release1Branch]);
            fixture.AssertFullSemver("1.2.0-alpha.2"); // why +2 not +1??
            fixture.AssertFullSemver("1.2.0-alpha.1", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());

            // Feature 2
            fixture.BranchTo(feature2Branch);
            fixture.MakeACommit("added feature 2");
            fixture.AssertFullSemver("1.2.0-f2.1+2"); // I see two commits why three?
            fixture.Checkout(developBranch);
            fixture.MergeNoFF(feature2Branch);
            fixture.Repository.Branches.Remove(fixture.Repository.Branches[feature2Branch]);
            fixture.AssertFullSemver("1.2.0-alpha.4"); // why +4 not +3??
            fixture.AssertFullSemver("1.2.0-alpha.3", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());

            // Release 1.2.0
            fixture.BranchTo(release2Branch);
            fixture.MakeACommit("release stabilization");
            fixture.AssertFullSemver("1.2.0-beta.1+1");
            fixture.Checkout(MainBranch);
            fixture.MergeNoFF(release2Branch);
            fixture.AssertFullSemver("1.2.0+0"); // why +0 and not +5??
            fixture.AssertFullSemver("1.1.1+5", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());
            fixture.ApplyTag("1.2.0");
            fixture.AssertFullSemver("1.2.0");
            fixture.Checkout(developBranch);
            fixture.MergeNoFF(release2Branch);
            fixture.Repository.Branches.Remove(fixture.Repository.Branches[release2Branch]);
            fixture.AssertFullSemver("1.3.0-alpha.2"); // why +2 and +1??
            fixture.AssertFullSemver("1.3.0-alpha.1", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());

            // Hotfix
            fixture.Checkout(MainBranch);
            fixture.BranchTo(hotfixBranch);
            fixture.MakeACommit("added hotfix");
            fixture.AssertFullSemver("1.2.1-beta.1+1"); // why seven it is just one commit on the hotfix branch since last rlease 1.2.0?
            fixture.AssertFullSemver("1.2.1-beta.1+1", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());
            fixture.Checkout(MainBranch);
            fixture.MergeNoFF(hotfixBranch);
            fixture.AssertFullSemver("1.2.1+2");
            fixture.ApplyTag("1.2.1");
            fixture.AssertFullSemver("1.2.1");
            fixture.Checkout(developBranch);
            fixture.MergeNoFF(hotfixBranch);
            fixture.Repository.Branches.Remove(fixture.Repository.Branches[hotfixBranch]);
            fixture.AssertFullSemver("1.3.0-alpha.9"); // why +9 and not +3??
            // two changes in hotfix and one merge commit
            fixture.AssertFullSemver("1.3.0-alpha.3", ConfigBuilder.New.WithoutAnyTrackMergeTargets().Build());
        }
    }
}
