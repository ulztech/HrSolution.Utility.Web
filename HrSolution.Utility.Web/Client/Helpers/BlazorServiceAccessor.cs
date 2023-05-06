﻿namespace HrSolution.Utility.Web.Client.Helpers
{
    internal sealed class BlazorServiceAccessor
    {
        private static readonly AsyncLocal<BlazorServiceHolder> s_currentServiceHolder = new();

        public IServiceProvider? Services
        {
            get => s_currentServiceHolder.Value?.Services;
            set
            {
                if (s_currentServiceHolder.Value is { } holder)
                {
                    // Clear the current IServiceProvider trapped in the AsyncLocal.
                    holder.Services = null;
                }

                if (value is not null)
                {
                    // Use object indirection to hold the IServiceProvider in an AsyncLocal
                    // so it can be cleared in all ExecutionContexts when it's cleared.
                    s_currentServiceHolder.Value = new() { Services = value };
                }
            }
        }

        private sealed class BlazorServiceHolder
        {
            public IServiceProvider? Services { get; set; }
        }
    }
}
