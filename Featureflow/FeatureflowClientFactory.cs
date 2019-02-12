using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    /// <summary>
    /// Featureflow client factory
    /// </summary>
    public static class FeatureflowClientFactory
    {
        /// <summary>
        /// Sync Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <returns>Featureflow client</returns>
        public static IFeatureflowClient Create(string apiKey)
        {
            return Create(apiKey, null, null);
        }

        /// <summary>
        /// Sync Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <returns>Featureflow client</returns>
        public static IFeatureflowClient Create(string apiKey, IEnumerable<Feature> defaultFeatures)
        {
            return Create(apiKey, defaultFeatures, null);
        }

        /// <summary>
        /// Sync Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <returns>Featureflow client</returns>
        public static IFeatureflowClient Create(string apiKey, FeatureflowConfig featureflowConfig)
        {
            return Create(apiKey, null, featureflowConfig);
        }

        /// <summary>
        /// Sync Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <returns>Featureflow client</returns>
        public static IFeatureflowClient Create(string apiKey, IEnumerable<Feature> defaultFeatures, FeatureflowConfig featureflowConfig)
        {
            return FeatureflowClient.Create(apiKey, defaultFeatures, featureflowConfig);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(string apiKey)
        {
            return CreateAsync(apiKey, CancellationToken.None);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(string apiKey, CancellationToken cancellationToken)
        {
            return CreateAsync(apiKey, null, null, cancellationToken);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(string apiKey, IEnumerable<Feature> defaultFeatures)
        {
            return CreateAsync(apiKey, defaultFeatures, null, CancellationToken.None);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            IEnumerable<Feature> defaultFeatures,
            CancellationToken cancellationToken)
        {
            return CreateAsync(apiKey, defaultFeatures, null, cancellationToken);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            FeatureflowConfig featureflowConfig)
        {
            return CreateAsync(apiKey, null, featureflowConfig, CancellationToken.None);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            FeatureflowConfig featureflowConfig,
            CancellationToken cancellationToken)
        {
            return CreateAsync(apiKey, null, featureflowConfig, cancellationToken);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            IEnumerable<Feature> defaultFeatures,
            FeatureflowConfig featureflowConfig)
        {
            return CreateAsync(apiKey, defaultFeatures, featureflowConfig, CancellationToken.None);
        }

        /// <summary>
        /// Async Featureflow client creation
        /// </summary>
        /// <param name="apiKey">Featureflow API key</param>
        /// <param name="defaultFeatures">Default features</param>
        /// <param name="featureflowConfig">Featureflow client configuration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Featureflow client</returns>
        public static Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            IEnumerable<Feature> defaultFeatures,
            FeatureflowConfig featureflowConfig,
            CancellationToken cancellationToken)
        {
            return FeatureflowClient.CreateAsync(apiKey, defaultFeatures, featureflowConfig, cancellationToken);
        }
    }
}
