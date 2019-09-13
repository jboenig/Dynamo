using System;

namespace Headway.AppCore.Repository
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRepositoryManager
	{
		/// <summary>
		/// Create a repository.
		/// </summary>
		/// <param name="repoName">Name of the repository to create.</param>
		/// <param name="repoTypeId">Type of repository to create.</param>
		RepositoryResultCode CreateRepository(string repoName, Guid repoTypeId);

		/// <summary>
		/// Test to see if the specified repository exists.
		/// </summary>
		/// <param name="repoName">Name of repository to check.</param>
		/// <returns></returns>
		bool RepositoryExists(string repoName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="repoName"></param>
		/// <returns></returns>
		Repository GetRepository(string repoName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="repoName"></param>
		void DeleteRepository(string repoName);
	}
}
