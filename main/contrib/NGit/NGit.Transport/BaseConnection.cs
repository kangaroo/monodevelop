/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Base helper class for implementing operations connections.</summary>
	/// <remarks>Base helper class for implementing operations connections.</remarks>
	/// <seealso cref="BasePackConnection">BasePackConnection</seealso>
	/// <seealso cref="BaseFetchConnection">BaseFetchConnection</seealso>
	public abstract class BaseConnection : Connection
	{
		private IDictionary<string, Ref> advertisedRefs = Sharpen.Collections.EmptyMap<string, Ref>();

		private bool startedOperation;

		private TextWriter messageWriter;

		public virtual IDictionary<string, Ref> GetRefsMap()
		{
			return advertisedRefs;
		}

		public ICollection<Ref> GetRefs()
		{
			return advertisedRefs.Values;
		}

		public Ref GetRef(string name)
		{
			return advertisedRefs.Get(name);
		}

		public virtual string GetMessages()
		{
			return messageWriter != null ? messageWriter.ToString() : string.Empty;
		}

		public abstract void Close();

		/// <summary>Denote the list of refs available on the remote repository.</summary>
		/// <remarks>
		/// Denote the list of refs available on the remote repository.
		/// <p>
		/// Implementors should invoke this method once they have obtained the refs
		/// that are available from the remote repository.
		/// </remarks>
		/// <param name="all">
		/// the complete list of refs the remote has to offer. This map
		/// will be wrapped in an unmodifiable way to protect it, but it
		/// does not get copied.
		/// </param>
		protected internal virtual void Available(IDictionary<string, Ref> all)
		{
			advertisedRefs = Sharpen.Collections.UnmodifiableMap(all);
		}

		/// <summary>Helper method for ensuring one-operation per connection.</summary>
		/// <remarks>
		/// Helper method for ensuring one-operation per connection. Check whether
		/// operation was already marked as started, and mark it as started.
		/// </remarks>
		/// <exception cref="NGit.Errors.TransportException">if operation was already marked as started.
		/// 	</exception>
		protected internal virtual void MarkStartedOperation()
		{
			if (startedOperation)
			{
				throw new TransportException(JGitText.Get().onlyOneOperationCallPerConnectionIsSupported
					);
			}
			startedOperation = true;
		}

		/// <summary>Get the writer that buffers messages from the remote side.</summary>
		/// <remarks>Get the writer that buffers messages from the remote side.</remarks>
		/// <returns>writer to store messages from the remote.</returns>
		protected internal virtual TextWriter GetMessageWriter()
		{
			if (messageWriter == null)
			{
				SetMessageWriter(new StringWriter());
			}
			return messageWriter;
		}

		/// <summary>Set the writer that buffers messages from the remote side.</summary>
		/// <remarks>Set the writer that buffers messages from the remote side.</remarks>
		/// <param name="writer">
		/// the writer that messages will be delivered to. The writer's
		/// <code>toString()</code>
		/// method should be overridden to return the
		/// complete contents.
		/// </param>
		protected internal virtual void SetMessageWriter(TextWriter writer)
		{
			if (messageWriter != null)
			{
				throw new InvalidOperationException(JGitText.Get().writerAlreadyInitialized);
			}
			messageWriter = writer;
		}
	}
}
