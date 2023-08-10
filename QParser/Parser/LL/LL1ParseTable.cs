using System.Collections;
using System.Collections.Generic;
using FluentResults;
using QParser.Lexer;

namespace QParser.Parser.LL;

public class LL1ParseTable : IEnumerable<(Rule rule, int tokenType, CompositeNonterminal compositeNonterminal)>
{
    private readonly Dictionary<Rule, Dictionary<int, CompositeNonterminal>> _ruleTokenDictionary = new();
    private readonly Dictionary<int, Dictionary<Rule, CompositeNonterminal>> _tokenRuleDictionary = new();

    public CompositeNonterminal this[Rule rule, int tokenType]
    {
        get => _ruleTokenDictionary[rule][tokenType];
        set
        {
            if (!_ruleTokenDictionary.ContainsKey(rule))
                _ruleTokenDictionary.Add(rule, new Dictionary<int, CompositeNonterminal>());
            _ruleTokenDictionary[rule][tokenType] = value;
            if (!_tokenRuleDictionary.ContainsKey(tokenType))
                _tokenRuleDictionary.Add(tokenType, new Dictionary<Rule, CompositeNonterminal>());
            _tokenRuleDictionary[tokenType][rule] = value;
        }
    }

    public IEnumerator<(Rule rule, int tokenType, CompositeNonterminal compositeNonterminal)> GetEnumerator()
    {
        foreach (var (rule, tokenDictionary) in _ruleTokenDictionary)
        foreach (var (tokenType, compositeNonterminal) in tokenDictionary)
            yield return (rule, tokenType, compositeNonterminal);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Rule rule, int tokenType, CompositeNonterminal compositeNonterminal)
    {
        this[rule, tokenType] = compositeNonterminal;
    }

    public bool TryAdd(Rule rule, int tokenType, CompositeNonterminal compositeNonterminal)
    {
        if (_ruleTokenDictionary.TryGetValue(rule, out var value) && value.ContainsKey(tokenType))
            return this[rule, tokenType] == compositeNonterminal;

        this[rule, tokenType] = compositeNonterminal;
        return true;
    }

    public bool TryGet(Rule rule, int tokenType, out CompositeNonterminal? compositeNonterminal)
    {
        if (_ruleTokenDictionary.TryGetValue(rule, out var value) &&
            value.TryGetValue(tokenType, out compositeNonterminal))
            return true;
        compositeNonterminal = null;
        return false;
    }


    public Result GenerateFromGrammar(Grammar grammar)
    {
        var result = Result.Ok();
        foreach (var rule in grammar.Rules)
        foreach (var subRule in rule.SubRules)
        {
            foreach (var first in subRule.First)
                if (!TryAdd(rule, first, subRule))
                    result.WithError(
                        $"M[{rule}, {first}] = {subRule} overlaps {this[rule, first]} by sharing the same FIRST");

            if (subRule.First.Contains(TokenConstants.Epsilon))
                foreach (var follow in rule.Follow)
                    if (!TryAdd(rule, follow, subRule))
                        result.WithError(
                            $"M[{rule}, {follow}] = {subRule} overlaps {this[rule, follow]} by sharing the same FOLLOW");
        }

        return result;
    }
}