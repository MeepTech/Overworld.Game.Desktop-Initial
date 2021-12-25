using Meep.Tech.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A collection of Values indexed by multiple non-unique tags
/// </summary>
/// <typeparam name="TTag">Assumed to have a unique hash code</typeparam>
/// <typeparam name="TValue">The stored values</typeparam>
public class TagedCollection<TTag, TValue> : IEnumerable<KeyValuePair<IEnumerable<TTag>, TValue>> {

  Dictionary<TTag, HashSet<TValue>> _valuesByTag
    = new Dictionary<TTag, HashSet<TValue>>();

  Dictionary<TValue, HashSet<TTag>> _tagsByValue
    = new Dictionary<TValue, HashSet<TTag>>();

  #region Get

  /// <summary>
  /// All distinct values
  /// </summary>
  public IEnumerable<TValue> Values
    => _tagsByValue.Keys;

  /// <summary>
  /// All distinct tags
  /// </summary>
  public IEnumerable<TTag> Tags
    => _valuesByTag.Keys;

  public IEnumerable<TValue> this[TTag tag] {
    get => _valuesByTag[tag];
  }

  #endregion

  #region Add

  /// <summary>
  /// Add a new value with as many tags as you want
  /// </summary>
  public void Add(TValue value, params TTag[] tags)
    => Add(tags, value);

  /// <summary>
  /// Add a new value with multiple tags
  /// </summary>
  public void Add(IEnumerable<TTag> tags, TValue value) {
    if(tags.Any()) {
      tags.ForEach(tag => {
        _valuesByTag.AddToHashSet(tag, value);
        _tagsByValue.AddToHashSet(value, tag);
      });
    } else
      throw new System.ArgumentException($"Tags cannot be null for provided value: {value?.ToString() ?? "null"}");
  }

  #endregion

  #region Remove

  /// <summary>
  /// Remove a value
  /// </summary>
  public bool Remove(TValue value) {
    if(_tagsByValue.ContainsKey(value)) {
      _tagsByValue.Remove(value);
      _valuesByTag.Values.ForEach(values => values.Remove(value));

      return true;
    }

    return false;
  }

  /// <summary>
  /// Remove all values for the given tag
  /// </summary>
  public bool RemoveValuesFor(TTag tag) {
    if(_valuesByTag.ContainsKey(tag)) {
      _valuesByTag.Remove(tag);
      _tagsByValue.Values.ForEach(values => values.Remove(tag));

      return true;
    }

    return false;
  }

  #endregion

  #region Find

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  public IEnumerable<TValue> FindWeightedMatches(int weightMultiplier, params TTag[] orderedTags)
    => FindWeightedMatches(orderedTags, weightMultiplier);

  /// <summary>
  /// Find the best matches, taking into account tag order with the default weight multiplier
  /// </summary>
  public IEnumerable<TValue> FindWeightedMatches(params TTag[] orderedTags)
    => FindWeightedMatches((IEnumerable<TTag>)orderedTags);

  /// <summary>
  /// Find the best matches, taking into account tag order with the default weight multiplier
  /// </summary>
  public IEnumerable<TValue> FindWeightedMatches(IEnumerable<TTag> orderedTags, int weightMultiplier = 2) {
    Dictionary<TValue, int> valueWeights = new();
    int weight = orderedTags.Count() * weightMultiplier;
    foreach(TTag tag in orderedTags) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + weight;
          } else
            valueWeights[value] = weight;
        });
      }
      weight--;
      weight--;
    }

    return _sortByWeight(valueWeights);
  }

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  public IEnumerable<TValue> FindWeightedMatches(params (TTag tag, int weight)[] @params)
    => FindWeightedMatches((IEnumerable<(TTag tag, int weight)>)@params);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  public IEnumerable<TValue> FindWeightedMatches(IEnumerable<(TTag tag, int weight)> @params) {
    Dictionary<TValue, int> valueWeights = new();
    foreach((TTag tag, int weight) in @params) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + weight;
          } else
            valueWeights[value] = weight;
        });
      }
    }

    return _sortByWeight(valueWeights);
  }

  /// <summary>
  /// Find the values that match the most tags in order
  /// </summary>
  public IEnumerable<TValue> FindBestMatches(params TTag[] tags)
    => FindWeightedMatches((IEnumerable<TTag>)tags);

  /// <summary>
  /// Find the values that match the most tags in order
  /// </summary>
  public IEnumerable<TValue> FindBestMatches(IEnumerable<TTag> orderedTags) {
    Dictionary<TValue, int> valueWeights = new();
    foreach(TTag tag in orderedTags) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight++;
          } else
            valueWeights[value] = 1;
        });
      }
    }

    return _sortByWeight(valueWeights);
  }

  #endregion

  #region Select

  /// <summary>
  /// Find the values that match any of the tags, unordered
  /// </summary>
  public TagedCollection<TTag, TValue> SelectMatches(params TTag[] tags)
    => SelectMatches((IEnumerable<TTag>)tags);

  /// <summary>
  /// Find the values that match any of the tags, unordered
  /// </summary>
  public TagedCollection<TTag, TValue> SelectMatches(IEnumerable<TTag> tags) {
    TagedCollection<TTag, TValue> @return = new();
    tags
      .ForEach(tag => _valuesByTag[tag]
      .ForEach(value => 
        @return.Add(_tagsByValue[value], value)));

    return @return;
  }

  /// <summary>
  /// Find the first value with the tags, or a default one with the best match
  /// </summary>
  public TValue FirstWithTagsOrDefault(params TTag[] tags)
    => FirstWithTagsOrDefault((IEnumerable<TTag>)tags);

  /// <summary>
  /// Find the first value with the tags, or a default one with the best match
  /// </summary>
  public TValue FirstWithTagsOrDefault(IEnumerable<TTag> tags) {
    TValue @return = SelectMatches(tags).Values.FirstOrDefault();
    if(@return is null) {
      return GetAllSortedByWeight(tags).FirstOrDefault();
    }

    return @return;
  }

  /// <summary>
  /// Select the best matches, taking into account tag order
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectWeightedMatches(int weightMultiplier, params TTag[] orderedTags)
    => SelectWeightedMatches(orderedTags, weightMultiplier);

  /// <summary>
  /// Select the best matches, taking into account tag order with the default weight multiplier
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectWeightedMatches(params TTag[] orderedTags)
    => SelectWeightedMatches((IEnumerable<TTag>)orderedTags);

  /// <summary>
  /// Select the best matches, taking into account tag order with the default weight multiplier.
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectWeightedMatches(IEnumerable<TTag> orderedTags, int weightMultiplier = 2) {
    Dictionary<TValue, int> valueWeights = new();
    int weight = orderedTags.Count() * weightMultiplier;
    foreach(TTag tag in orderedTags) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + weight;
          } else
            valueWeights[value] = weight;
        });
      }
      weight--;
      weight--;
    }

    TagedCollection<TTag, TValue> @return = new();
    _sortByWeight(valueWeights).ForEach(value => {
      @return.Add(_tagsByValue[value], value);
    });

    return @return;
  }

  /// <summary>
  /// Select matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectWeightedMatches(params (TTag tag, int weight)[] @params)
    => SelectWeightedMatches((IEnumerable<(TTag tag, int weight)>)@params);

  /// <summary>
  /// Select matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectWeightedMatches(IEnumerable<(TTag tag, int weight)> @params) {
    Dictionary<TValue, int> valueWeights = new();
    foreach((TTag tag, int weight) in @params) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + weight;
          } else
            valueWeights[value] = weight;
        });
      }
    }

    TagedCollection<TTag, TValue> @return = new();
    _sortByWeight(valueWeights).ForEach(value => {
      @return.Add(_tagsByValue[value], value);
    });
    return @return;
  }

  /// <summary>
  /// Select the values that match the most tags in order
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectBestMatches(params TTag[] tags)
    => SelectWeightedMatches((IEnumerable<TTag>)tags);

  /// <summary>
  /// Select the values that match the most tags in order
  /// Slower than Find due to the cast
  /// </summary>
  public TagedCollection<TTag, TValue> SelectBestMatches(IEnumerable<TTag> orderedTags) {
    Dictionary<TValue, int> valueWeights = new();
    foreach(TTag tag in orderedTags) {
      if(_valuesByTag.TryGetValue(tag, out var values)) {
        values.ForEach(value => {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight++;
          } else
            valueWeights[value] = 1;
        });
      }
    }

    TagedCollection<TTag, TValue> @return = new();
    _sortByWeight(valueWeights).ForEach(value => {
      @return.Add(_tagsByValue[value], value);
    });
    return @return;
  }

  #endregion

  #region All Sorted

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(int weightMultiplier, params TTag[] orderedTags)
    => GetAllSortedByWeight(orderedTags, weightMultiplier);

  /// <summary>
  /// Find the best matches, taking into account tag order with the default weight multiplier
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(params TTag[] orderedTags)
    => GetAllSortedByWeight(orderedTags.ToList());

  /// <summary>
  /// Find the best matches, taking into account tag order with the default weight multiplier
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(IEnumerable<TTag> orderedTags)
    => GetAllSortedByWeight(orderedTags.ToList());

  /// <summary>
  /// Find the best matches, taking into account tag order with the default weight multiplier
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(IList<TTag> orderedTags, int weightMultiplier = 2) {
    Dictionary<TValue, int> valueWeights = new();
    Dictionary<TTag, int> tagIndexes = new();
    int maxWeight = orderedTags.Count() * weightMultiplier;
    List<TValue> remainders = new();
    foreach(TValue value in Values) {
      IEnumerable<TTag> tags = _tagsByValue[value].Intersect(orderedTags);
      if(!tags.Any()) {
        remainders.Add(value);
      } else
        foreach(TTag tag in tags) {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight 
              + (maxWeight - (tagIndexes[tag] = orderedTags.IndexOf(tag))) * weightMultiplier;
          } else
            valueWeights[value] = tagIndexes.TryGetValue(tag, out int tagIndex)
              ? (maxWeight - tagIndex) * weightMultiplier
              : (maxWeight - (tagIndexes[tag] = orderedTags.IndexOf(tag))) * weightMultiplier;
        }
    }

    return _sortByWeight(valueWeights)
      .Concat(remainders);
  }

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(params (TTag tag, int weight)[] @params)
    => GetAllSortedByWeight((IEnumerable<(TTag tag, int weight)>)@params);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByWeight(IEnumerable<(TTag tag, int weight)> @params) {
    Dictionary<TValue, int> valueWeights = new();
    Dictionary<TTag, int> tagWeights = @params.ToDictionary(
      v => v.tag,
      v => v.weight
    );
    List<TValue> remainders = new();
    foreach(TValue value in Values) {
      IEnumerable<TTag> tags = _tagsByValue[value].Intersect(tagWeights.Keys);
      if(!tags.Any()) {
        remainders.Add(value);
      } else
        foreach(TTag tag in tags) {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + tagWeights[tag];
          } else
            valueWeights[value] = tagWeights[tag];
        }
    }

    return _sortByWeight(valueWeights)
      .Concat(remainders);
  }

  /// <summary>
  /// Find the values that match the most tags
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByBestMatch(params TTag[] tags)
    => GetAllSortedByBestMatch((IEnumerable<TTag>)tags);

  /// <summary>
  /// Find the values that match the most tags
  /// </summary>
  public IEnumerable<TValue> GetAllSortedByBestMatch(IEnumerable<TTag> orderedTags) {
    Dictionary<TValue, int> valueWeights = new();
    List<TValue> remainders = new();
    foreach(TValue value in Values) {
      IEnumerable<TTag> tags = _tagsByValue[value].Intersect(orderedTags);
      if(!tags.Any()) {
        remainders.Add(value);
      } else
        foreach(TTag tag in tags) {
          if(valueWeights.TryGetValue(value, out var existingWeight)) {
            valueWeights[value] = existingWeight + 1;
          } else
            valueWeights[value] = 1;
        }
    }

    return _sortByWeight(valueWeights)
      .Concat(remainders);
  }

  #endregion

  public IEnumerator<KeyValuePair<IEnumerable<TTag>, TValue>> GetEnumerator()
    => _tagsByValue.Select(e => new KeyValuePair<IEnumerable<TTag>, TValue>(e.Value, e.Key)).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

  static IEnumerable<TValue> _sortByWeight(Dictionary<TValue, int> valueWeights)
    => valueWeights
          .OrderByDescending(e => e.Value)
          .Select(e => e.Key);
}