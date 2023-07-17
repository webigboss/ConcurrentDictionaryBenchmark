using System.Text;

namespace ConcurrentDictionaryBenchmark
{
    /// <summary>
    /// Trie that will find Ngram strings and return values of type <typeparamref name="TValue"/> for each sequence found.
    /// <see ref="https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm"/> for the original concept.
    /// The Compressed trie data structure <see href="http://www.mathcs.emory.edu/~cheung/Courses/253/Syllabus/Text/trie02.html"/> has been applied to above concept
    /// for reduced memory consumption.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that will be returned when the sequence is found.</typeparam>
    public sealed class CompressedNgramTrie<TValue>
    {
        /// <summary>
        /// Root of the trie. It has no value and no parent.
        /// </summary>
        private readonly Node root;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressedNgramTrie{TValue}"/> class. 
        /// </summary>
        public CompressedNgramTrie()
        {
            this.root = new Node();
        }

        /// <summary>
        /// Create a Trie from a tuple enumerable.
        /// </summary>
        /// <param name="tuples">The tuple enumerable contains the string to be added and associated values.</param>
        /// <returns> The <see cref="CompressedNgramTrie{TValue}"/> built from the tuple enumerable.</returns>
        public static CompressedNgramTrie<TValue> Create(IEnumerable<Tuple<string, TValue>> tuples)
        {
            var trie = new CompressedNgramTrie<TValue>();

            foreach (var tuple in tuples)
            {
                trie.Add(tuple);
            }

            return trie;
        }

        /// <summary>
        /// Search in the input and find all matched sub sequences.
        /// </summary>
        /// <param name="input">The input sequence to search in.</param>
        /// <returns>The values that were added for the found sequences.</returns>
        public IEnumerable<TValue> Search(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            var node = this.root;

            for (int inputIndex = 0, matchIndex = 0; inputIndex < input.Length;)
            {
                if (matchIndex == input.Length || !node.Children.TryGetValue(input[matchIndex], out Tuple<Node, StringBuilder> tuple))
                {
                    inputIndex++;
                    matchIndex = inputIndex;
                    node = this.root;

                    continue;
                }

                node = tuple.Item1;
                var edge = tuple.Item2;

                int edgeIndex = 0;
                while (edgeIndex < edge.Length &&
                        matchIndex < input.Length &&
                        edge[edgeIndex] == input[matchIndex])
                {
                    edgeIndex++;
                    matchIndex++;
                }

                if (edgeIndex == edge.Length && node.IsEnd)
                {
                    yield return node.Value;
                }
            }
        }

        /// <summary>
        /// Splits the edge at the current node. Pushes the Node down and inserts a new parent node in the middle.
        /// </summary>
        /// <param name="node">Current node.</param>
        /// <param name="edge">Current edge.</param>
        /// <param name="edgeIndex">Edge index at which split needs to happen.</param>
        private static void SplitEdge(Node node, StringBuilder edge, int edgeIndex)
        {
            var childNodeTuple = CreateChildTuple(edge.ToString(edgeIndex, edge.Length - edgeIndex));
            var childNode = childNodeTuple.Item2;

            if (node.IsEnd)
            {
                childNode.IsEnd = node.IsEnd;
                childNode.Value = node.Value;
            }

            foreach (var child in node.Children)
            {
                childNode.Children.Add(child);
            }

            node.Children.Clear();
            node.Children.Add(childNodeTuple.Item1, Tuple.Create(childNode, childNodeTuple.Item3));
            edge.Length = edgeIndex;
        }

        /// <summary>
        /// Adds a new child node to the current node.
        /// </summary>
        /// <param name="node">Current Node.</param>
        /// <param name="edge">Edge for the child node.</param>
        /// <param name="value">Associated value with the Node.</param>
        private static void AddChild(Node node, string edge, TValue value)
        {
            var childNodeTuple = CreateChildTuple(edge);
            var childNode = childNodeTuple.Item2;

            childNode.IsEnd = true;
            childNode.Value = value;

            node.Children.Add(childNodeTuple.Item1, Tuple.Create(childNode, childNodeTuple.Item3));
        }

        /// <summary>
        /// Creates the Child Tuple for the new node.
        /// </summary>
        /// <param name="edge">Edge string</param>
        /// <returns>Tuple for the new node.</returns>
        private static Tuple<char, Node, StringBuilder> CreateChildTuple(string edge)
        {
            var childNodeKey = edge[0];
            var childStringBuilder = new StringBuilder(edge, edge.Length);

            return Tuple.Create(childNodeKey, new Node(), childStringBuilder);
        }

        /// <summary>
        /// Adds a sequence to the trie tree.
        /// </summary>
        /// <param name="inputTuple">string/value tuple to be added to Trie.</param>
        private void Add(Tuple<string, TValue> inputTuple)
        {
            var node = this.root;
            var input = inputTuple.Item1;
            var value = inputTuple.Item2;

            if (string.IsNullOrEmpty(input))
            {
                throw new InvalidOperationException("Null/Empty sequence exists when adding to a trie.");
            }

            for (int inputIndex = 0; inputIndex < input.Length;)
            {
                // If the first letter of the substring is not present in the Trie, add the full substring as a single Node and exit.
                if (!node.Children.TryGetValue(input[inputIndex], out Tuple<Node, StringBuilder> tuple))
                {
                    var childNode = new Node() { IsEnd = true, Value = value };
                    var edgeLength = input.Length - inputIndex;
                    var stringBuilder = new StringBuilder(input, inputIndex, edgeLength, edgeLength);

                    node.Children.Add(input[inputIndex], Tuple.Create(childNode, stringBuilder));

                    return;
                }

                node = tuple.Item1;
                var edge = tuple.Item2;

                // Traverse edge till the substring matches the edge.
                int edgeIndex = 0;
                while (edgeIndex < edge.Length &&
                        inputIndex < input.Length &&
                        edge[edgeIndex] == input[inputIndex])
                {
                    edgeIndex++;
                    inputIndex++;
                }

                // Edge fully matched with rest of the substring so mark the node as Word end and exit.
                // Trie:
                //      root
                //      | (edge: face)
                //      node (face)
                // new string to add "face"
                if (edgeIndex == edge.Length && inputIndex == input.Length)
                {
                    if (node.IsEnd)
                    {
                        throw new InvalidOperationException("Duplicated sequences exist when adding to trie");
                    }

                    node.IsEnd = true;
                    node.Value = value;
                    return;
                }

                // Edge string is exhausted and substring is still pending so move to next level.
                // Trie:
                //      root
                //      | (edge: face)
                //      node (face)
                // new string to add "facebook"
                //      root
                //      | (edge: face)
                //      node (face)
                //      | (edge: book)
                //      node (book)
                if (edgeIndex == edge.Length)
                {
                    continue;
                }

                // Input substring is exhausted but Edge string is still pending so split the edge, add a new node and exit
                // Trie:
                //      root
                //      | (edge: facebook)
                //      node (facebook)
                // new string to add "face"
                //      root
                //      | (edge: face)
                //      node (face)
                //      | (edge: book)
                //      node (book)
                if (inputIndex == input.Length)
                {
                    SplitEdge(node, edge, edgeIndex);
                    node.Value = value;
                    return;
                }

                // Neither Input nor Edge is exhausted so split the edge, push partial edge down and add a new node.
                // Trie:
                //              root
                //               | (edge: facebook)
                //              node (facebook)
                // new string to add "facepalm"
                //                 root
                //                  | (edge: face)
                //              node (face)
                // (edge:palm)     /    \ (edge: book)
                //       node (palm)     node (book)
                SplitEdge(node, edge, edgeIndex);
                node.IsEnd = false;
                AddChild(node, input.Substring(inputIndex), value);

                return;
            }
        }

        /// <summary>
        /// Internal Trie Node class
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Boolean to denote if current Node defines an end of string.
            /// </summary>
            private bool isEnd;

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            public Node()
            {
                this.Children = new Dictionary<char, Tuple<Node, StringBuilder>>();
                this.isEnd = false;
            }

            /// <summary>
            /// Gets dictionary of children of the current Node.
            /// </summary>
            public IDictionary<char, Tuple<Node, StringBuilder>> Children { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether gets or Sets the isEnd property.
            /// </summary>
            public bool IsEnd
            {
                get
                {
                    return this.isEnd;
                }

                set
                {
                    if (!value)
                    {
                        this.Value = default(TValue);
                    }

                    this.isEnd = value;
                }
            }

            /// <summary>
            /// Gets or sets value associated with the current node.
            /// </summary>
            public TValue Value { get; set; }
        }
    }
}
